using common.libs;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using common.libs.extends;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Concurrent;

namespace smash.proxy.server
{
    public sealed class ProxyClient
    {
        private readonly ProxyClientConfig proxyClientConfig;

        private Socket Socket;
        private UdpClient UdpClient;
        private ConcurrentDictionary<IPEndPoint, ProxyUserToken> udpConnections = new ConcurrentDictionary<IPEndPoint, ProxyUserToken>();

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
            acceptEventArg.UserToken = new ProxyUserToken
            {
                Saea = acceptEventArg,
                Step = Socks5EnumStep.Request,
                Request = new ProxyInfo
                {
                    BufferSize = proxyClientConfig.BufferSize,
                    AddressType = Socks5EnumAddressType.IPV4
                }
            };
            acceptEventArg.Completed += IO_Completed;
            StartAccept(acceptEventArg);

            IAsyncResult result = UdpClient.BeginReceive(ProcessReceiveUdp, new ProxyUserToken
            {
                Step = Socks5EnumStep.ForwardUdp,
                PoolBuffer = new byte[(1 << (byte)proxyClientConfig.BufferSize) * 1024],
                Request = new ProxyInfo
                {
                    BufferSize = proxyClientConfig.BufferSize,
                    AddressType = Socks5EnumAddressType.IPV4
                }
            });
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

                ProxyUserToken token = new ProxyUserToken
                {
                    ClientSocket = socket,
                    Step = Socks5EnumStep.Request,
                    Request = new ProxyInfo
                    {
                        BufferSize = proxyClientConfig.BufferSize,
                        AddressType = Socks5EnumAddressType.IPV4,
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
                token.PoolBuffer = new byte[(1 << (byte)proxyClientConfig.BufferSize) * 1024];
                readEventArgs.SetBuffer(token.PoolBuffer, 0, (1 << (byte)proxyClientConfig.BufferSize) * 1024);
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


        private async Task<bool> ConnectServer(ProxyUserToken token, ProxyInfo info)
        {
            try
            {
                Socket socket = new Socket(proxyClientConfig.ServerEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(proxyClientConfig.ServerEP);
                SslStream sslStream = new SslStream(new NetworkStream(socket), true, ValidateServerCertificate, null);
                await sslStream.AuthenticateAsClientAsync(string.Empty);

                token.ServerStream = sslStream;
                byte[] bytes = info.PackConnect(proxyClientConfig.KeyMemory, out int length);
                info.Data = bytes.AsMemory(0, length);
                await Request(token);
                info.Return(bytes);
                BindServerReceive(token);

                return true;
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"connect server -> error " + ex);
            }
            return false;
        }
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private void BindServerReceive(ProxyUserToken token)
        {
            if (token.ServerStream == null || token.ServerStream.CanRead == false) return;

            token.ServerPoolBuffer = new byte[(1 << (byte)proxyClientConfig.BufferSize) * 1024];
            token.ServerStream.BeginRead(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length, ServerReceiveCallback, token);
        }
        private async void ServerReceiveCallback(IAsyncResult result)
        {
            ProxyUserToken token = result.AsyncState as ProxyUserToken;
            try
            {
                int length = token.ServerStream.EndRead(result);

                token.Request.Data = token.ServerPoolBuffer.AsMemory(0, length);
                await InputData(token, token.Request);

                token.ServerStream.BeginRead(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length, ServerReceiveCallback, token);
            }
            catch (Exception ex)
            {
                CloseClientSocket(token);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }


        private async Task<bool> ReceiveCommandData(ProxyUserToken token, SocketAsyncEventArgs e, int totalLength)
        {
            EnumProxyValidateDataResult validate = ValidateData(token);
            if ((validate & EnumProxyValidateDataResult.TooShort) == EnumProxyValidateDataResult.TooShort)
            {
                //太短
                while ((validate & EnumProxyValidateDataResult.TooShort) == EnumProxyValidateDataResult.TooShort)
                {
                    totalLength += await token.ClientSocket.ReceiveAsync(e.Buffer.AsMemory(e.Offset + totalLength), SocketFlags.None);
                    token.Request.Data = e.Buffer.AsMemory(e.Offset, totalLength);
                    validate = ValidateData(token);
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
        private EnumProxyValidateDataResult ValidateData(ProxyUserToken token)
        {
            return token.Step switch
            {
                Socks5EnumStep.Request => Socks5Parser.ValidateRequestData(token.Request.Data),
                Socks5EnumStep.Command => Socks5Parser.ValidateCommandData(token.Request.Data),
                Socks5EnumStep.Auth => Socks5Parser.ValidateAuthData(token.Request.Data, Socks5EnumAuthType.Password),
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
                if (token.Step <= Socks5EnumStep.Command)
                {
                    bool canNext = await ReceiveCommandData(token, e, totalLength);
                    if (canNext == false) return;

                }

                await Receive(token);

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
                        await Receive(token);
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
                Logger.Instance.Error(ex);
            }
        }

        private async void ProcessReceiveUdp(IAsyncResult result)
        {
            IPEndPoint rep = null;
            try
            {
                ProxyUserToken receiveToken = result.AsyncState as ProxyUserToken;

                receiveToken.Request.Data = UdpClient.EndReceive(result, ref rep);
                receiveToken.Request.SourceEP = rep;

                if (udpConnections.TryGetValue(rep, out ProxyUserToken token) == false)
                {
                    token = new ProxyUserToken
                    {
                        Request = new ProxyInfo
                        {
                            BufferSize = receiveToken.Request.BufferSize,
                            AddressType = receiveToken.Request.AddressType,
                            SourceEP = receiveToken.Request.SourceEP,
                            Data = receiveToken.Request.Data
                        },
                        Step = receiveToken.Step,
                    };
                    udpConnections.TryAdd(rep, token);
                    GetRemoteEndPoint(token.Request, out int index);
                    if (await ConnectServer(token, token.Request) == false)
                    {
                        result = UdpClient.BeginReceive(ProcessReceiveUdp, token);
                        return;
                    }
                }
                else
                {
                    token.Request.Data = receiveToken.Request.Data;
                    token.Request.SourceEP = receiveToken.Request.SourceEP = rep;
                }

                if (token.Request.Data.Length > 0)
                {
                    if (await HandleRequestData(token, token.Request))
                    {
                        await Request(token);
                        token.Request.Data = Helper.EmptyArray;
                    }
                }
                result = UdpClient.BeginReceive(ProcessReceiveUdp, token);
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"listen udp -> error " + ex);
            }
        }


        private async Task Receive(ProxyUserToken token)
        {
            try
            {
                if (token.Request.Data.Length == 0 && token.Step == Socks5EnumStep.Command)
                {
                    return;
                }
                if (await HandleRequestData(token, token.Request) == false)
                {
                    return;
                }

                bool res = await Request(token);
                if (res == false)
                {
                    if (token.Step == Socks5EnumStep.Command)
                    {
                        await InputData(token, token.Request);
                    }
                    else if (token.Step == Socks5EnumStep.Forward)
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
            await token.ServerStream.WriteAsync(token.Request.Data);
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

                Socks5EnumStep step = token.Step;
                bool commandAndFail = step == Socks5EnumStep.Command && (Socks5EnumResponseCommand)info.Data.Span[0] != Socks5EnumResponseCommand.ConnecSuccess;

                if (HandleAnswerData(token, info))
                {
                    if (token.Step == Socks5EnumStep.ForwardUdp)
                    {
                        //组装udp包
                        byte[] bytes = Socks5Parser.MakeUdpResponse(new IPEndPoint(new IPAddress(info.TargetAddress.Span), info.TargetPort), info.Data, out int legnth);
                        await UdpClient.SendAsync(info.Data, info.SourceEP);
                        Socks5Parser.Return(bytes);
                    }
                    else
                    {
                        await token.ClientSocket.SendAsync(info.Data, SocketFlags.None).AsTask().WaitAsync(TimeSpan.FromSeconds(5));
                    }
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
        private async Task<bool> HandleRequestData(ProxyUserToken token, ProxyInfo info)
        {
            //request  auth 的 直接通过,跳过验证部分
            if (token.Step < Socks5EnumStep.Command)
            {
                info.Data = new byte[] { 0x00 };
                token.Step++;
                await InputData(token, info);
                return false;
            }
            //command 的
            if (token.Step == Socks5EnumStep.Command)
            {
                Socks5EnumRequestCommand command = (Socks5EnumRequestCommand)info.Data.Span[1];
                //将socks5的command转化未通用command
                if (command == Socks5EnumRequestCommand.Bind)
                {
                    info.Data = new byte[] { (byte)Socks5EnumResponseCommand.CommandNotAllow };
                    await InputData(token, info);
                    return false;
                }


                if (command == Socks5EnumRequestCommand.Connect)
                {
                    //解析出目标地址
                    GetRemoteEndPoint(info, out int index);
                    if (await ConnectServer(token, info) == false)
                    {
                        info.Data = new byte[] { (byte)Socks5EnumResponseCommand.NetworkError };
                        await InputData(token, info);
                        return false;
                    }
                }

                info.Data = new byte[] { (byte)Socks5EnumResponseCommand.ConnecSuccess };
                await InputData(token, info);
                return false;
            }
            else
            {
                if (token.Step == Socks5EnumStep.ForwardUdp)
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
        private bool HandleAnswerData(ProxyUserToken token, ProxyInfo info)
        {
            //request auth 步骤的，只需回复一个字节的状态码
            if (token.Step < Socks5EnumStep.Command)
            {
                info.Data = new byte[] { 5, info.Data.Span[0] };
                token.Step++;
                return true;
            }

            switch (token.Step)
            {
                case Socks5EnumStep.Command:
                    {
                        //command的，需要区分成功和失败，成功则回复指定数据，失败则关闭连接
                        info.Data = Socks5Parser.MakeConnectResponse(new IPEndPoint(IPAddress.Any, proxyClientConfig.ListenPort), info.Data.Span[0]);
                        token.Step = Socks5EnumStep.Forward;
                    }
                    break;
                case Socks5EnumStep.Forward:
                    {
                        token.Step = Socks5EnumStep.Forward;
                    }
                    break;
                case Socks5EnumStep.ForwardUdp:
                    {
                        token.Step = Socks5EnumStep.ForwardUdp;
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
            if (token.Request.SourceEP != null)
            {
                udpConnections.TryRemove(token.Request.SourceEP, out _);
            }
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
                UdpClient?.Dispose();
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
    }

    sealed class ProxyUserToken
    {
        public Socket ClientSocket { get; set; }
        public SocketAsyncEventArgs Saea { get; set; }
        public byte[] PoolBuffer { get; set; }
        public ProxyInfo Request { get; set; } = new ProxyInfo { };
        public Socks5EnumStep Step { get; set; } = Socks5EnumStep.Request;

        public SslStream ServerStream { get; set; }
        public byte[] ServerPoolBuffer { get; set; }

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

        public EnumBufferSize BufferSize { get; set; } = EnumBufferSize.KB_8;
    }
}
