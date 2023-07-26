using common.libs.extends;
using System.Net;
using System;
using System.Net.Sockets;
using common.libs;
using System.Threading.Tasks;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace smash.proxy.server
{
    public sealed class ProxyServer
    {
        private readonly ProxyServerConfig proxyServerConfig;
        private Socket Socket;

        public ProxyServer(ProxyServerConfig proxyServerConfig)
        {
            this.proxyServerConfig = proxyServerConfig;
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
                Memory<byte> data = e.Buffer.AsMemory(e.Offset, e.BytesTransferred);
                if (token.Step <= Socks5EnumStep.Command)
                {
                    await ConnectTarget(token, data);

                }
                else
                {
                    await SendToRemote(token, data);
                }

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
                        data = e.Buffer.AsMemory(0, length);
                        await SendToRemote(token, data);

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
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }

        private async Task SendToRemote(ProxyServerUserToken token, Memory<byte> data)
        {
            /*
            if (token.IsProxy)
            {
                do
                {
                    //有数据长度，那就只剩下两种情况，要么只读到了content-length，但是未读到 \r\n\r\n，要么都读到了，但是数据未读完
                    if (token.LastLength > 0)
                    {
                        //数据未读完
                        if (token.HeaderEnd)
                        {
                            Memory<byte> sendDta = data;
                            if (data.Length > token.LastLength)
                            {
                                token.LastLength = 0;
                                sendDta = sendDta.Slice(token.LastLength);
                            }
                            else
                            {
                                token.LastLength -= sendDta.Length;
                            }
                            data = data.Slice(sendDta.Length);
                        }
                        else //读一下http header 结束位置
                        {
                            int index = HttpParser.GetHeaderEndIndex(data);
                            token.HeaderEnd = index >= 0;
                            if (token.HeaderEnd)
                            {
                                data = data.Slice(index + 4); //\r\n\r\n 4个长度
                            }
                            else
                            {
                                int length = await token.ClientSocket.ReceiveAsync(data.Slice(data.Length), SocketFlags.None);
                            }
                        }
                    }
                    else //没长度，就说明没读到Content-Length
                    {


                        int length = await token.ClientSocket.ReceiveAsync(data, SocketFlags.None);
                    }

                } while (data.Length > 0);

                return;
            }
            */
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
                ProxyInfo info = new ProxyInfo();
                token.TargerEP = proxyServerConfig.FakeEP;
                if (info.ValidateKey(data, proxyServerConfig.KeyMemory))
                {
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
                    Command = token.Command
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

                    await token.ClientSocket.SendAsync(e.Buffer.AsMemory(offset, length), SocketFlags.None);

                    if (token.TargetSocket.Available > 0)
                    {
                        while (token.TargetSocket.Available > 0)
                        {
                            length = await token.TargetSocket.ReceiveAsync(e.Buffer.AsMemory(), SocketFlags.None);
                            if (length > 0)
                            {
                                await token.ClientSocket.SendAsync(e.Buffer.AsMemory(0, length), SocketFlags.None);
                            }
                        }
                    }

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
                    await token.ClientSocket.SendAsync(token.PoolBuffer.AsMemory(0, length), SocketFlags.None);
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
                        //Console.WriteLine($"read domain:{info.TargetAddress.GetString()}");
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

        public IPEndPoint FakeEP { get; set; }

        public EnumBufferSize BufferSize { get; set; } = EnumBufferSize.KB_8;
    }
}


