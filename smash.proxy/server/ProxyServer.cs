using common.libs.extends;
using System.Net;
using System;
using System.Net.Sockets;
using common.libs;
using System.Threading.Tasks;
using System.Text;

namespace smash.proxy.server
{
    public sealed class ProxyServer
    {
        private readonly ProxyServerConfig proxyServerConfig;
        private Socket Socket;

        private ICrypto crypto;
        public ProxyServer(ProxyServerConfig proxyServerConfig)
        {
            this.proxyServerConfig = proxyServerConfig;
            crypto = new CryptoFactory().CreateSymmetric("12345678901234567890123456789011");
        }

        public bool Start()
        {
            BindAccept();
            return true;
        }

        private void BindAccept()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, proxyServerConfig.ListenPort);
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(endpoint);
            Socket.Listen(int.MaxValue);

            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.UserToken = new ProxyServerUserToken
            {
                Saea = acceptEventArg,
                Step = Socks5EnumStep.Command,
            };
            acceptEventArg.Completed += IO_Completed;
            StartAccept(acceptEventArg);
        }
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {

                acceptEventArg.AcceptSocket = null;
                ProxyServerUserToken token = ((ProxyServerUserToken)acceptEventArg.UserToken);
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
                ProxyServerUserToken acceptToken = (e.UserToken as ProxyServerUserToken);

                ProxyServerUserToken token = new ProxyServerUserToken
                {
                    ClientSocket = socket,
                    Step = Socks5EnumStep.Command,
                };

                SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs
                {
                    UserToken = token,
                    SocketFlags = SocketFlags.None
                };
                token.Saea = readEventArgs;

                token.ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, true);
                token.ClientSocket.SendTimeout = 5000;
                token.PoolBuffer = new byte[(1 << (byte)proxyServerConfig.BufferSize) * 1024];
                readEventArgs.SetBuffer(token.PoolBuffer, 0, (1 << (byte)proxyServerConfig.BufferSize) * 1024);
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
        private async void ProcessReceive(SocketAsyncEventArgs e)
        {
            ProxyServerUserToken token = (ProxyServerUserToken)e.UserToken;
            try
            {
                if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
                {
                    CloseClientSocket(e);
                    return;
                }


                int totalLength = e.BytesTransferred;
                Memory<byte> data = e.Buffer.AsMemory(0, e.BytesTransferred);
                Console.WriteLine($"ProcessReceive:{token.ClientSocket.GetHashCode()}:{token.Step}:{token.IsProxy}:{data.Length}");
                if (token.Step <= Socks5EnumStep.Command)
                {
                    await ConnectTarget(token, data);

                }
                else
                {
                    await SendToRemote(e, token, data);
                }
                if (token.ClientSocket.Connected == false)
                {
                    CloseClientSocket(e);
                    return;
                }
                Console.WriteLine($"ProcessReceive:start receive");
                if (token.ClientSocket.ReceiveAsync(e) == false)
                {
                    ProcessReceive(e);
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(e);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }

        private async Task SendToRemote(SocketAsyncEventArgs e, ProxyServerUserToken token, Memory<byte> data)
        {
            if (token.IsProxy)
            {
                int lastLength = token.LastLength;
                bool headed = token.HeaderEnd;
                data = HttpParser.GetContentData(data, e.Buffer, ref lastLength, ref headed, out int offset);
                token.LastLength = lastLength;
                token.HeaderEnd = headed;
                if (offset != e.Offset)
                {
                    e.SetBuffer(e.Buffer, offset, e.Buffer.Length);
                }

                if (data.Length == 0) return;
            }
            if (token.Command == Socks5EnumRequestCommand.Connect)
            {
                await token.TargetSocket.SendAsync(data, SocketFlags.None);
            }
            else
            {
                await token.TargetSocket.SendToAsync(data, SocketFlags.None, token.TargerEP);
            }
        }

        private async Task<bool> ConnectTarget(ProxyServerUserToken token, Memory<byte> data)
        {
            try
            {
                Console.WriteLine($"ConnectTarget:{token.ClientSocket.GetHashCode()}");
                ProxyInfo info = new ProxyInfo();
                token.TargerEP = proxyServerConfig.FakeEP;
                if (info.ValidateKey(data, proxyServerConfig.KeyMemory))
                {
                    Console.WriteLine("ConnectTarget proxy");
                    info.UnPackConnect(data);
                    token.TargerEP = ReadRemoteEndPoint(info);
                    token.IsProxy = true;
                }

                token.Command = info.Command;
                ProxyServerUserToken proxyServerUserToken = new ProxyServerUserToken
                {
                    ClientSocket = token.ClientSocket,
                    PoolBuffer = new byte[(1 << (byte)proxyServerConfig.BufferSize) * 1024],
                    Step = Socks5EnumStep.Forward,
                    TargerEP = token.TargerEP,
                    TargetSocket = token.TargetSocket,
                    Command = token.Command,
                    IsProxy = token.IsProxy
                };

                if (info.Command == Socks5EnumRequestCommand.Connect)
                {
                    token.TargetSocket = new Socket(token.TargerEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    proxyServerUserToken.TargetSocket = token.TargetSocket;
                    await proxyServerUserToken.TargetSocket.ConnectAsync(proxyServerUserToken.TargerEP).WaitAsync(TimeSpan.FromSeconds(5));
                    if (data.Length > 0 && proxyServerUserToken.TargerEP.Equals(proxyServerConfig.FakeEP))
                    {
                        await proxyServerUserToken.TargetSocket.SendAsync(data);
                    }
                    BindTargetReceive(proxyServerUserToken);
                }
                else
                {
                    token.TargetSocket = new Socket(token.TargerEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    token.TargetSocket.EnableBroadcast = true;
                    token.TargetSocket.WindowsUdpBug();

                    proxyServerUserToken.TargetSocket = token.TargetSocket;
                    proxyServerUserToken.PoolBuffer = new byte[65535];
                    token.TargetSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                    IAsyncResult result = token.TargetSocket.BeginReceiveFrom(proxyServerUserToken.PoolBuffer, 0, proxyServerUserToken.PoolBuffer.Length, SocketFlags.None, ref proxyServerUserToken.TempRemoteEP, ReceiveCallbackUdp, proxyServerUserToken);

                }

                token.Step = Socks5EnumStep.Forward;
                await Response(proxyServerUserToken, Encoding.UTF8.GetBytes("success"));
                return true;
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"connect server -> error " + ex);

                CloseClientSocket(token);
            }
            return false;
        }
        private void Target_IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    TargetProcessReceive(e);
                    break;
                default:
                    break;
            }
        }
        private void BindTargetReceive(ProxyServerUserToken token)
        {
            SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs
            {
                UserToken = token,
                SocketFlags = SocketFlags.None,
            };
            token.Saea = readEventArgs;
            token.PoolBuffer = new byte[(1 << (byte)proxyServerConfig.BufferSize) * 1024];
            readEventArgs.SetBuffer(token.PoolBuffer, 0, (1 << (byte)proxyServerConfig.BufferSize) * 1024);
            readEventArgs.Completed += Target_IO_Completed;

            if (token.TargetSocket.ReceiveAsync(readEventArgs) == false)
            {
                TargetProcessReceive(readEventArgs);
            }
        }
        private async void TargetProcessReceive(SocketAsyncEventArgs e)
        {
            ProxyServerUserToken token = (ProxyServerUserToken)e.UserToken;
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    int offset = e.Offset;
                    int length = e.BytesTransferred;

                    await Response(token, e.Buffer.AsMemory(0, length));
                    if (token.TargetSocket.Connected == false)
                    {
                        CloseClientSocket(e);
                        return;
                    }
                    if (token.TargetSocket.ReceiveAsync(e) == false)
                    {
                        TargetProcessReceive(e);
                    }
                }
                else
                {
                    CloseClientSocket(e);
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(e);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private async void ReceiveCallbackUdp(IAsyncResult result)
        {
            ProxyServerUserToken token = result.AsyncState as ProxyServerUserToken;
            try
            {
                int length = token.TargetSocket.EndReceiveFrom(result, ref token.TempRemoteEP);
                if (length > 0)
                {
                    await Response(token, token.PoolBuffer.AsMemory(0, length));
                }
                result = token.TargetSocket.BeginReceiveFrom(token.PoolBuffer, 0, token.PoolBuffer.Length, SocketFlags.None, ref token.TempRemoteEP, ReceiveCallbackUdp, token);
            }
            catch (Exception ex)
            {
                CloseClientSocket(token);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"socks5 forward udp -> receive" + ex);
            }
        }
        private async Task Response(ProxyServerUserToken token, Memory<byte> data)
        {
            if (token.IsProxy)
            {
                byte[] bytes = ProxyInfo.PackResponse(proxyServerConfig.HttpResponseHeader, data, out int length);
                Console.WriteLine($"response:{Encoding.UTF8.GetString(bytes)}");
                await token.ClientSocket.SendAsync(bytes.AsMemory(0, length), SocketFlags.None);
                ProxyInfo.ReturnStatic(bytes);
            }
            else
            {
                await token.ClientSocket.SendAsync(data, SocketFlags.None);
            }
        }
        private IPEndPoint ReadRemoteEndPoint(ProxyInfo info)
        {
            IPAddress ip = IPAddress.Any;
            switch (info.AddressType)
            {
                case Socks5EnumAddressType.IPV4:
                case Socks5EnumAddressType.IPV6:
                    {
                        ip = new IPAddress(info.TargetAddress.Span);
                    }
                    break;
                case Socks5EnumAddressType.Domain:
                    {
                        Console.WriteLine($"read domain:{info.TargetAddress.GetString()}");
                        ip = NetworkHelper.GetDomainIp(info.TargetAddress.GetString());
                    }
                    break;
                default:
                    break;
            }
            return new IPEndPoint(ip, info.TargetPort);
        }


        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            ProxyServerUserToken token = e.UserToken as ProxyServerUserToken;
            CloseClientSocket(token);
        }
        private void CloseClientSocket(ProxyServerUserToken token)
        {
            token.ClientSocket?.SafeClose();
            token.TargetSocket?.SafeClose();
            token.PoolBuffer = null;
            token.Saea?.Dispose();
            GC.Collect();
        }
        public void Stop()
        {
            try
            {
                Socket?.SafeClose();
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
    }

    public sealed class ProxyServerUserToken
    {
        public Socket ClientSocket { get; set; }
        public SocketAsyncEventArgs Saea { get; set; }
        public byte[] PoolBuffer { get; set; }
        public Socks5EnumStep Step { get; set; } = Socks5EnumStep.Command;
        public Socks5EnumRequestCommand Command { get; set; }

        public Socket TargetSocket { get; set; }

        //udp
        public IPEndPoint TargerEP { get; set; }
        public EndPoint TempRemoteEP = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);

        //解包
        public bool IsProxy { get; set; }
        public bool HeaderEnd { get; set; }
        public int LastLength { get; set; }

    }

    public sealed class ProxyServerConfig
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

        public Memory<byte> HttpResponseHeader { get; set; } = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: ");

        public IPEndPoint FakeEP { get; set; }

        public EnumBufferSize BufferSize { get; set; } = EnumBufferSize.KB_8;
    }
}


