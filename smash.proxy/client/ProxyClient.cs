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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace smash.proxy.server
{
    public sealed class ProxyClient
    {
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
            var endpoint = new IPEndPoint(IPAddress.Any, proxyClientConfig.ListenPort);
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
                    RequestId = proxyClientConfig.ListenPort
                }
            };
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
                Socket socket = e.AcceptSocket;
                ProxyUserToken acceptToken = (e.UserToken as ProxyUserToken);

                uint id = requestIdNs.Increment();
                ProxyUserToken token = new ProxyUserToken
                {
                    ClientSocket = socket,
                    Request = new ProxyInfo
                    {
                        RequestId = id,
                        BufferSize = EnumBufferSize.KB_8,
                        Command = Socks5EnumRequestCommand.Connect,
                        AddressType = Socks5EnumAddressType.IPV4,
                        Step = Socks5EnumStep.Request
                    },
                };

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


        private async Task<SslStream> ConnectServer()
        {
            try
            {
                Socket socket = new Socket(proxyClientConfig.ServerEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(proxyClientConfig.ServerEP);
                SslStream sslStream = new SslStream(new NetworkStream(socket), true, ValidateServerCertificate, null);
                await sslStream.AuthenticateAsClientAsync(string.Empty);

                return sslStream;
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"connect server -> error " + ex);
            }
            return null;
        }
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private void BindServerReceive(ProxyUserToken token)
        {
            if (token.ServerStream == null || token.ServerStream.CanRead == false) return;

            token.ServerPoolBuffer = new byte[(1 << (byte)EnumBufferSize.KB_8) * 1024];
            token.ServerStream.BeginRead(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length, ServerReceiveCallback, token);
        }
        private void ServerReceiveCallback(IAsyncResult result)
        {
            ProxyUserToken token = result.AsyncState as ProxyUserToken;
            int length = token.ServerStream.EndRead(result);
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
            token.Request.Data = data;
            await Receive(token, data);
        }
        private async Task Receive(ProxyUserToken token)
        {
            try
            {
                if (token.Request.Data.Length == 0 && token.Request.Step == Socks5EnumStep.Command)
                {
                    return;
                }
                if (await HandleRequestData(token) == false)
                {
                    return;
                }

                if (token.ServerStream == null)
                {
                    await ConnectServer();
                    BindServerReceive(token);
                }

                bool res = await Request(token);
                if (res == false)
                {
                    if (token.Request.Step == Socks5EnumStep.Command)
                    {
                        await InputData(token, token.Request);
                    }
                    else if (token.Request.Step == Socks5EnumStep.Forward)
                    {
                        CloseClientSocket(token);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
        }
        private async Task<bool> Request(ProxyUserToken token)
        {
            if (token.ServerStream == null || token.ServerStream.CanWrite == false)
            {
                return false;
            }
            byte[] bytes = token.Request.ToBytes(proxyClientConfig.KeyMemory, out int length);
            await token.ServerStream.WriteAsync(bytes.AsMemory(0,length));
            token.Request.Return(bytes);
            return true;
        }

        private async Task InputData(ProxyUserToken token, ProxyInfo info)
        {
            try
            {
                if (info.Data.Length == 0)
                {
                    CloseClientSocket(token);
                    return;
                }

                Socks5EnumStep step = info.Step;
                bool commandAndFail = step == Socks5EnumStep.Command && info.CommandStatus != Socks5EnumResponseCommand.ConnecSuccess;

                HandleAnswerData(info);
                token.Request.Step = info.Step;
                token.Request.Command = info.Command;
                token.Request.CommandStatus = info.CommandStatus;

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
                    CloseClientSocket(token);
                    return;
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(token);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private async Task<bool> HandleRequestData(ProxyUserToken token)
        {
            //request  auth 的 直接通过,跳过验证部分
            if (token.Request.Step < Socks5EnumStep.Command)
            {
                token.Request.Data = new byte[] { 0x00 };
                token.Request.Step++;
                await InputData(token, token.Request);
                return false;
            }
            //command 的
            if (token.Request.Step == Socks5EnumStep.Command)
            {
                //解析出目标地址
                GetRemoteEndPoint(token.Request, out int index);
                //udp中继的时候，有可能是 0.0.0.0:0 直接通过
                if (token.Request.TargetAddress.GetIsAnyAddress())
                {
                    token.Request.Data = new byte[] { (byte)Socks5EnumResponseCommand.ConnecSuccess };
                    await InputData(token, token.Request);
                    return false;
                }

                //将socks5的command转化未通用command
                token.Request.Command = (Socks5EnumRequestCommand)token.Request.Data.Span[1];
                token.Request.Data = token.Request.Data.Slice(index);
            }
            else
            {
                if (token.Request.Step == Socks5EnumStep.ForwardUdp)
                {
                    //解析出目标地址
                    GetRemoteEndPoint(token.Request, out int index);
                    //解析出udp包的数据部分
                    token.Request.Data = Socks5Parser.GetUdpData(token.Request.Data);
                }

                if (token.Request.TargetPort == 53)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Debug($"[DNS查询]:{string.Join(",", token.Request.Data.ToArray())}:{Encoding.UTF8.GetString(info.Data.ToArray())}");
                }
            }
            if (token.Request.TargetAddress.GetIsAnyAddress()) return false;

            return true;
        }
        private bool HandleAnswerData(ProxyInfo info)
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
                        info.Data = Socks5Parser.MakeConnectResponse(new IPEndPoint(IPAddress.Any, proxyClientConfig.ListenPort), (byte)info.CommandStatus);
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
            token.ClientSocket?.SafeClose();
            token.ServerStream?.Dispose();
            token.PoolBuffer = null;
            token.ServerPoolBuffer = null;
            token.Request = null;
            token.Saea?.Dispose();
            GC.Collect();
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

        public SslStream ServerStream { get; set; }
        public byte[] ServerPoolBuffer { get; set; }
        public ProxyInfo Response { get; set; } = new ProxyInfo { };

    }

    public sealed class ProxyClientConfig
    {
        public ushort ListenPort { get; set; }
        public string Key
        {
            set
            {
                keyMemory = value.ToBytes();
            }
        }
        private Memory<byte> keyMemory;
        public Memory<byte> KeyMemory { get => keyMemory; }

        public IPEndPoint ServerEP { get; set; }
    }
}
