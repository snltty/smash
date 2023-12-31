﻿using common.libs.extends;
using System.Net;
using System;
using System.Net.Sockets;
using common.libs;
using System.Threading.Tasks;
using System.Text;
using smash.proxy.protocol;
using common.libs.socks5;

namespace smash.proxy.server
{
    internal sealed class ProxyServer
    {
        private readonly ProxyServerConfig proxyServerConfig;
        private Socket Socket;
        private Random random = new Random();

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
                token.ClientSocket.KeepAlive();
                token.PoolBuffer = new byte[(1 << (byte)proxyServerConfig.BufferSize) * 1024];
                readEventArgs.SetBuffer(token.PoolBuffer, 0, token.PoolBuffer.Length);
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
                Memory<byte> data = e.Buffer.AsMemory(0, totalLength);
                if (token.Step <= Socks5EnumStep.Command)
                {
                    await ConnectTarget(token, data);
                }
                else
                {
                    await SendToRemote(token, data);
                    while (token.ClientSocket.Available > 0)
                    {
                        totalLength = await token.ClientSocket.ReceiveAsync(e.Buffer.AsMemory(e.Offset));
                        if (totalLength > 0)
                        {
                            data = e.Buffer.AsMemory(0, totalLength);
                            await SendToRemote(token, data);
                        }
                    }
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
            IPEndPoint target = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                ProxyInfo info = new ProxyInfo();
                token.TargerEP = proxyServerConfig.FakeEP;
                if (info.ValidateKey(data, proxyServerConfig.KeyMemory))
                {
                    info.UnPackConnect(data, proxyServerConfig.KeyMemory);
                    token.TargerEP = ReadRemoteEndPoint(info);
                    token.IsProxy = true;
                    if (token.TargerEP.Port == 53 && (info.TargetAddress.Span[0] == 192 || info.TargetAddress.Span[0] == 172))
                    {
                        token.TargerEP.Address = proxyServerConfig.Dns;
                    }

                }
                else
                {
                    info.Data = data;
                }
                token.Command = info.Command;
                token.Step = Socks5EnumStep.Forward;
                if (info.Command == Socks5EnumRequestCommand.Connect)
                {
                    token.TargetSocket = new Socket(token.TargerEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    token.TargetSocket.KeepAlive();
                    token.TargetSocket = token.TargetSocket;
                    target = token.TargerEP;
                    await token.TargetSocket.ConnectAsync(token.TargerEP).WaitAsync(TimeSpan.FromSeconds(5));

                    BindTargetReceive(token);
                    if (info.Data.Length > 0)
                    {
                        await token.TargetSocket.SendAsync(info.Data);
                    }
                }
                else
                {
                    token.TargetSocket = new Socket(token.TargerEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    token.TargetSocket.WindowsUdpBug();
                    if (token.TargerEP.AddressFamily == AddressFamily.InterNetwork)
                    {
                        token.TempRemoteEP = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);
                        token.TargetSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                    }
                    else
                    {
                        token.TempRemoteEP = new IPEndPoint(IPAddress.IPv6Any, IPEndPoint.MinPort);
                        token.TargetSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
                    }
                    if (info.Data.Length > 0)
                    {
                        await token.TargetSocket.SendToAsync(info.Data, SocketFlags.None, token.TargerEP);
                    }
                    BindTargetReceiveUdp(token);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"connect server -> error {target} " + ex);

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
            token.TargetSaea = readEventArgs;
            token.TargetPoolBuffer = new byte[(1 << (byte)proxyServerConfig.BufferSize) * 1024];
            readEventArgs.SetBuffer(token.TargetPoolBuffer, 0, token.TargetPoolBuffer.Length);
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
                    int length = e.BytesTransferred;

                    await Response(token, e.Buffer.AsMemory(0, length));
                    while (token.TargetSocket.Available > 0)
                    {
                        length = await token.TargetSocket.ReceiveAsync(e.Buffer.AsMemory(0));
                        if (length > 0)
                        {
                            await Response(token, e.Buffer.AsMemory(0, length));
                        }
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
        private async Task Response(ProxyServerUserToken token, Memory<byte> data)
        {
            Memory<byte> memory = data;
            byte[] bytes = Helper.EmptyArray;
            try
            {
                if (token.FirstPack == false)
                {
                    token.FirstPack = true;
                    if (token.IsProxy && memory.Length < 1024)
                    {
                        int padding = random.Next(128, 1024);
                        bytes = ProxyInfo.PackFirstResponse(data, padding, out int length);
                        memory = bytes.AsMemory(0, length);
                    }
                }
                await token.ClientSocket.SendAsync(memory, SocketFlags.None);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (bytes.Length > 0)
                {
                    ProxyInfo.ReturnStatic(bytes);
                }
            }
        }


        private void BindTargetReceiveUdp(ProxyServerUserToken token)
        {
            token.TargetPoolBuffer = new byte[65535];
            token.TargetSocket.BeginReceiveFrom(token.TargetPoolBuffer, 0, token.TargetPoolBuffer.Length, SocketFlags.None, ref token.TempRemoteEP, TargetProcessReceiveUDP, token);

        }
        private async void TargetProcessReceiveUDP(IAsyncResult result)
        {
            ProxyServerUserToken token = (result.AsyncState as ProxyServerUserToken);
            try
            {
                int length = token.TargetSocket.EndReceiveFrom(result, ref token.TempRemoteEP);
                await Response(token, token.TargetPoolBuffer.AsMemory(0, length));
                token.TargetSocket.BeginReceiveFrom(token.TargetPoolBuffer, 0, token.TargetPoolBuffer.Length, SocketFlags.None, ref token.TempRemoteEP, TargetProcessReceiveUDP, token);
            }
            catch (Exception ex)
            {
                CloseClientSocket(token);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
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
            token.TargetPoolBuffer = null;
            token.Saea?.Dispose();
            token.TargetSaea?.Dispose();
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
        public byte[] PoolBuffer { get; set; }
        public SocketAsyncEventArgs Saea { get; set; }
        public Socks5EnumStep Step { get; set; } = Socks5EnumStep.Command;
        public Socks5EnumRequestCommand Command { get; set; } = Socks5EnumRequestCommand.Connect;


        public Socket TargetSocket { get; set; }
        public byte[] TargetPoolBuffer { get; set; }
        public SocketAsyncEventArgs TargetSaea { get; set; }

        //udp
        public IPEndPoint TargerEP { get; set; }
        public EndPoint TempRemoteEP;

        //解包
        public bool IsProxy { get; set; }
        public bool FirstPack { get; set; }
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


        private IPAddress _dns = IPAddress.Any;
        public IPAddress Dns
        {
            get => _dns; set
            {
                _dns = value;
                DNSMemory = value.GetAddressBytes();
            }
        }
        public Memory<byte> DNSMemory { get; private set; }
    }
}


