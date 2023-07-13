using common.libs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using common.libs.extends;
using System.Text;

namespace smash.proxy.server
{
    public sealed class ProxyClient
    {
        private readonly ClientsManager clientsManager = new ClientsManager();
        private NumberSpaceUInt32 requestIdNs = new NumberSpaceUInt32(65536);
        private readonly ProxyClientConfig proxyClientConfig;

        private Socket Socket;
        private UdpClient UdpClient;

        public ProxyClient(ProxyClientConfig proxyClientConfig)
        {
            this.proxyClientConfig = proxyClientConfig;
        }

        public bool Start()
        {
            BindAccept();
            return true;
        }

        private void BindAccept()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, proxyClientConfig.Port);
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(endpoint);
            Socket.Listen(int.MaxValue);

            UdpClient = new UdpClient(endpoint);

            UdpClient.EnableBroadcast = true;
            UdpClient.Client.WindowsUdpBug();

            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
            ProxyUserToken token = new ProxyUserToken
            {
                Saea = acceptEventArg,
                Request = new ProxyInfo
                {
                    Step = Socks5EnumStep.ForwardUdp,
                    Command = Socks5EnumRequestCommand.UdpAssociate,
                    CommandStatus = Socks5EnumResponseCommand.ConnecSuccess,
                    BufferSize = EnumBufferSize.KB_8,
                    RequestId = proxyClientConfig.Port
                }
            };
            clientsManager.TryAdd(token);
            acceptEventArg.UserToken = token;
            acceptEventArg.Completed += IO_Completed;
            StartAccept(acceptEventArg);

            IAsyncResult result = UdpClient.BeginReceive(ProcessReceiveUdp, token);
        }
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {

                acceptEventArg.AcceptSocket = null;
                ProxyUserToken token = ((ProxyUserToken)acceptEventArg.UserToken);
                if (Socket.AcceptAsync(acceptEventArg) == false)
                {
                    ProcessAccept(acceptEventArg);
                }
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                default:
                    Logger.Instance.Error(e.LastOperation.ToString());
                    break;
            }
        }
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            BindReceive(e);
            StartAccept(e);
        }
        private void BindReceive(SocketAsyncEventArgs e)
        {
            try
            {
                ProxyUserToken acceptToken = (e.UserToken as ProxyUserToken);

                uint id = requestIdNs.Increment();
                ProxyUserToken token = new ProxyUserToken
                {
                    ClientSocket = e.AcceptSocket,
                    Request = new ProxyInfo
                    {
                        RequestId = id,
                        BufferSize = EnumBufferSize.KB_8,
                        Command = Socks5EnumRequestCommand.Connect,
                        AddressType = Socks5EnumAddressType.IPV4,
                        Step = Socks5EnumStep.Request
                    },
                };
                clientsManager.TryAdd(token);

                SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs
                {
                    UserToken = token,
                    SocketFlags = SocketFlags.None
                };
                token.Saea = readEventArgs;

                token.ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, true);
                token.ClientSocket.SendTimeout = 5000;
                token.PoolBuffer = new byte[(1 << (byte)EnumBufferSize.KB_8) * 1024];
                readEventArgs.SetBuffer(token.PoolBuffer, 0, (1 << (byte)EnumBufferSize.KB_8) * 1024);
                readEventArgs.Completed += IO_Completed;

                if (token.ClientSocket.ReceiveAsync(readEventArgs) == false)
                {
                    ProcessReceive(readEventArgs);
                }
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }

        private async Task<bool> ReceiveCommandData(ProxyUserToken token, SocketAsyncEventArgs e, int totalLength)
        {
            EnumProxyValidateDataResult validate = ValidateData(token.Request);
            if ((validate & EnumProxyValidateDataResult.TooShort) == EnumProxyValidateDataResult.TooShort)
            {
                //太短
                while ((validate & EnumProxyValidateDataResult.TooShort) == EnumProxyValidateDataResult.TooShort)
                {
                    totalLength += await token.ClientSocket.ReceiveAsync(e.Buffer.AsMemory(e.Offset + totalLength), SocketFlags.None);
                    token.Request.Data = e.Buffer.AsMemory(e.Offset, totalLength);
                    validate = ValidateData(token.Request);
                }
            }

            //不短，又不相等，直接关闭连接
            if ((validate & EnumProxyValidateDataResult.Equal) != EnumProxyValidateDataResult.Equal)
            {
                CloseClientSocket(e);
                return false;
            }
            return true;
        }
        private EnumProxyValidateDataResult ValidateData(ProxyInfo info)
        {
            return info.Step switch
            {
                Socks5EnumStep.Request => Socks5Parser.ValidateRequestData(info.Data),
                Socks5EnumStep.Command => Socks5Parser.ValidateCommandData(info.Data),
                Socks5EnumStep.Auth => Socks5Parser.ValidateAuthData(info.Data, Socks5EnumAuthType.Password),
                Socks5EnumStep.Forward => EnumProxyValidateDataResult.Equal,
                Socks5EnumStep.ForwardUdp => EnumProxyValidateDataResult.Equal,
                _ => EnumProxyValidateDataResult.Equal
            };
        }
        private async void ProcessReceive(SocketAsyncEventArgs e)
        {
            ProxyUserToken token = (ProxyUserToken)e.UserToken;
            try
            {
                if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
                {
                    CloseClientSocket(e);
                    return;
                }


                int totalLength = e.BytesTransferred;
                token.Request.Data = e.Buffer.AsMemory(e.Offset, e.BytesTransferred);
                //有些客户端，会把一个包拆开发送，很奇怪，不得不验证一下数据完整性
                if (token.Request.Step <= Socks5EnumStep.Command)
                {
                    bool canNext = await ReceiveCommandData(token, e, totalLength);
                    if (canNext == false) return;

                }

                await Receive(token.Request);

                if (token.ClientSocket.Available > 0)
                {
                    while (token.ClientSocket.Available > 0)
                    {
                        int length = await token.ClientSocket.ReceiveAsync(e.Buffer.AsMemory(), SocketFlags.None);
                        if (length == 0)
                        {
                            CloseClientSocket(e);
                            return;
                        }
                        token.Request.Data = e.Buffer.AsMemory(0, length);
                        await Receive(token.Request);
                    }
                }

                if (token.ClientSocket.Connected == false)
                {
                    CloseClientSocket(e);
                    return;
                }
                if (token.ClientSocket.ReceiveAsync(e) == false)
                {
                    ProcessReceive(e);
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(e);
                Logger.Instance.Error($"{token.Request.RequestId} {ex + ""}");
            }
        }
        private async void ProcessReceiveUdp(IAsyncResult result)
        {
            IPEndPoint rep = null;
            try
            {
                ProxyUserToken token = result.AsyncState as ProxyUserToken;

                token.Request.Data = UdpClient.EndReceive(result, ref rep);

                token.Request.SourceEP = rep;
                await Receive(token.Request);
                token.Request.Data = Helper.EmptyArray;

                result = UdpClient.BeginReceive(ProcessReceiveUdp, token);
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"listen udp -> error " + ex);
            }
        }

        private async Task Receive(ProxyUserToken token, Memory<byte> data)
        {
            await Receive(token.Request, data);
        }
        private async Task Receive(ProxyInfo info, Memory<byte> data)
        {
            info.Data = data;
            await Receive(info);
        }
        private async Task Receive(ProxyInfo info)
        {
            try
            {
                if (info.Data.Length == 0 && info.Step == Socks5EnumStep.Command)
                {
                    return;
                }
                if (await HandleRequestData(info) == false)
                {
                    return;
                }

                bool res = await Request(info);
                if (res == false)
                {
                    if (info.Step == Socks5EnumStep.Command)
                    {
                        await InputData(info);
                    }
                    else if (info.Step == Socks5EnumStep.Forward)
                    {
                        clientsManager.TryRemove(info.RequestId, out _);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
        }
        public async Task<bool> Request(ProxyInfo info)
        {
            return true;
        }

        public async Task InputData(ProxyInfo info)
        {
            try
            {
                if (info.Data.Length == 0)
                {
                    clientsManager.TryRemove(info.RequestId, out _);
                    return;
                }

                if (clientsManager.TryGetValue(info.RequestId, out ProxyUserToken token) == false)
                {
                    return;
                }

                Socks5EnumStep step = info.Step;
                bool commandAndFail = step == Socks5EnumStep.Command && info.CommandStatus != Socks5EnumResponseCommand.ConnecSuccess;

                HandleAnswerData(info);

                if (info.Step == Socks5EnumStep.ForwardUdp)
                {
                    await UdpClient.SendAsync(info.Data, info.SourceEP);
                }
                else
                {
                    await token.ClientSocket.SendAsync(info.Data, SocketFlags.None).AsTask().WaitAsync(TimeSpan.FromSeconds(5));
                }

                if (commandAndFail)
                {
                    clientsManager.TryRemove(info.RequestId, out _);
                    return;
                }
            }
            catch (Exception ex)
            {
                clientsManager.TryRemove(info.RequestId, out _);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        public async Task<bool> HandleRequestData(ProxyInfo info)
        {
            //request  auth 的 直接通过,跳过验证部分
            if (info.Step < Socks5EnumStep.Command)
            {
                info.Data = new byte[] { 0x00 };
                info.Step++;
                await InputData(info);
                return false;
            }
            //command 的
            if (info.Step == Socks5EnumStep.Command)
            {
                //解析出目标地址
                GetRemoteEndPoint(info, out int index);
                //udp中继的时候，有可能是 0.0.0.0:0 直接通过
                if (info.TargetAddress.GetIsAnyAddress())
                {
                    info.Data = new byte[] { (byte)Socks5EnumResponseCommand.ConnecSuccess };
                    await InputData(info);
                    return false;
                }

                //将socks5的command转化未通用command
                info.Command = (Socks5EnumRequestCommand)info.Data.Span[1];
                info.Data = info.Data.Slice(index);
            }
            else
            {
                if (info.Step == Socks5EnumStep.ForwardUdp)
                {
                    //解析出目标地址
                    GetRemoteEndPoint(info, out int index);
                    //解析出udp包的数据部分
                    info.Data = Socks5Parser.GetUdpData(info.Data);
                }

                if (info.TargetPort == 53)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Debug($"[DNS查询]:{string.Join(",", info.Data.ToArray())}:{Encoding.UTF8.GetString(info.Data.ToArray())}");
                }
            }
            if (info.TargetAddress.GetIsAnyAddress()) return false;

            return true;
        }
        public bool HandleAnswerData(ProxyInfo info)
        {
            //request auth 步骤的，只需回复一个字节的状态码
            if (info.Step < Socks5EnumStep.Command)
            {
                info.Data = new byte[] { 5, info.Data.Span[0] };
                info.Step++;
                return true;
            }

            switch (info.Step)
            {
                case Socks5EnumStep.Command:
                    {
                        //command的，需要区分成功和失败，成功则回复指定数据，失败则关闭连接
                        info.Data = Socks5Parser.MakeConnectResponse(new IPEndPoint(IPAddress.Any, proxyClientConfig.Port), (byte)info.CommandStatus);
                        info.Step = Socks5EnumStep.Forward;
                    }
                    break;
                case Socks5EnumStep.Forward:
                    {
                        info.Step = Socks5EnumStep.Forward;
                    }
                    break;
                case Socks5EnumStep.ForwardUdp:
                    {
                        //组装udp包
                        info.Data = Socks5Parser.MakeUdpResponse(new IPEndPoint(new IPAddress(info.TargetAddress.Span), info.TargetPort), info.Data);
                        info.Step = Socks5EnumStep.ForwardUdp;
                    }
                    break;
            }
            return true;
        }
        private void GetRemoteEndPoint(ProxyInfo info, out int index)
        {
            try
            {
                info.TargetAddress = Socks5Parser.GetRemoteEndPoint(info.Data, out Socks5EnumAddressType addressType, out ushort port, out index);
                info.AddressType = addressType;
                info.TargetPort = port;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                Logger.Instance.Error($"step:{info.Step},data:{string.Join(",", info.Data.ToArray())}");

                throw;
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            ProxyUserToken token = e.UserToken as ProxyUserToken;
            CloseClientSocket(token);
        }
        private void CloseClientSocket(ProxyUserToken token)
        {
            clientsManager.TryRemove(token.Request.RequestId, out _);
            _ = Receive(token, Helper.EmptyArray);
        }
        public void Stop()
        {
            try
            {
                Socket?.SafeClose();
                UdpClient?.Close();
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
            clientsManager.Clear();
        }
    }

    sealed class ProxyUserToken
    {
        public Socket ClientSocket { get; set; }
        public SocketAsyncEventArgs Saea { get; set; }
        public byte[] PoolBuffer { get; set; }
        public ProxyInfo Request { get; set; } = new ProxyInfo { };

        public Socket ServerSocket { get; set; }
    }

    sealed class ClientsManager
    {
        private ConcurrentDictionary<ulong, ProxyUserToken> clients = new();

        public bool TryAdd(ProxyUserToken model)
        {
            return clients.TryAdd(model.Request.RequestId, model);
        }
        public bool TryGetValue(ulong id, out ProxyUserToken c)
        {
            return clients.TryGetValue(id, out c);
        }
        public bool TryRemove(ulong id, out ProxyUserToken c)
        {
            bool res = clients.TryRemove(id, out c);
            if (res)
            {
                try
                {
                    c?.ClientSocket.SafeClose();
                    c?.ServerSocket.SafeClose();
                    c.PoolBuffer = Helper.EmptyArray;
                    c?.Saea.Dispose();
                    GC.Collect();
                    //  GC.SuppressFinalize(c);
                }
                catch (Exception ex)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Error(ex);
                }
            }
            return res;
        }
        public void Clear()
        {
            IEnumerable<ulong> requestIds = clients.Select(c => c.Key);
            foreach (var requestId in requestIds)
            {
                TryRemove(requestId, out _);
            }
        }
    }

    public sealed class ProxyClientConfig
    {
        public ushort Port { get; set; }
        public string Key { get; set; }
    }
}
