using common.libs.socks5;
using common.libs;
using smash.plugins.proxy;
using System.Buffers.Binary;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using common.libs.extends;
using System.Collections.Concurrent;

namespace smash.plugins.hijack
{
    public sealed class HijackServer
    {
        private readonly ConcurrentDictionary<ushort, byte[]> endpointMap = new ConcurrentDictionary<ushort, byte[]>();
        private readonly ConcurrentDictionary<ulong, UdpConnection> udpConnections = new ConcurrentDictionary<ulong, UdpConnection>();
        private Socket Socket;
        private readonly ProxyConfig proxyConfig;
        public HijackServer(ProxyConfig proxyConfig)
        {
            this.proxyConfig = proxyConfig;
        }

        public int Start()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, 0);
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(endpoint);
            Socket.Listen(int.MaxValue);

            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.Completed += IO_Completed;
            StartAccept(acceptEventArg);

            return (Socket.LocalEndPoint as IPEndPoint).Port;
        }

        #region TCP
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {
                acceptEventArg.AcceptSocket = null; ;
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
        private unsafe void BindReceive(SocketAsyncEventArgs e)
        {
            try
            {
                Socket socket = e.AcceptSocket;
                ProxyClientUserToken acceptToken = (e.UserToken as ProxyClientUserToken);
                IPEndPoint local = socket.RemoteEndPoint as IPEndPoint;
                if (endpointMap.TryRemove((ushort)local.Port, out byte[] remote) == false)
                {
                    socket.SafeClose();
                    return;
                }


                ProxyClientUserToken token = new ProxyClientUserToken
                {
                    ClientSocket = socket,
                    Remote = remote,
                };

                SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs
                {
                    UserToken = token,
                    SocketFlags = SocketFlags.None
                };
                token.Saea = readEventArgs;
                token.ClientSocket.KeepAlive();
                token.PoolBuffer = new byte[8 * 1024];
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
            ProxyClientUserToken token = (ProxyClientUserToken)e.UserToken;
            try
            {
                if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
                {
                    CloseClientSocket(e);
                    return;
                }

                int length = e.BytesTransferred;
                if (token.ServerSocket == null)
                {

                    ConnectServer(token);
                    if (token.ServerSocket == null)
                    {
                        return;
                    }
                }

                await token.ServerSocket.SendAsync(e.Buffer.AsMemory(0, length));

                if (token.ClientSocket.Available > 0)
                {
                    while (token.ClientSocket.Available > 0)
                    {
                        length = await token.ClientSocket.ReceiveAsync(e.Buffer.AsMemory(), SocketFlags.None);
                        if (length == 0)
                        {
                            CloseClientSocket(e);
                            return;
                        }
                        await token.ServerSocket.SendAsync(e.Buffer.AsMemory(0, length), SocketFlags.None);
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

        /// <summary>
        /// 缓存四元组对应关系，本机，所以不需要本机ip，端口即可
        /// </summary>
        /// <param name="localPort"></param>
        /// <param name="remote"></param>
        public void CacheEndPoint(ushort localPort, byte[] remote)
        {
            endpointMap.TryAdd(localPort, remote);
        }
        /// <summary>
        /// 连接socks5服务
        /// </summary>
        /// <param name="token"></param>
        private unsafe void ConnectServer(ProxyClientUserToken token)
        {
            fixed (void* p = token.Remote)
            {
                token.ServerSocket = CreateConnection((nint)p, Socks5EnumRequestCommand.Connect, out IPEndPoint serverEP);
                if (token.ServerSocket == null)
                {
                    CloseClientSocket(token);
                    return;
                }
            }
            BindReceiveServer(token);

        }
        /// <summary>
        /// socks5服务接收数据
        /// </summary>
        /// <param name="token"></param>
        private void BindReceiveServer(ProxyClientUserToken token)
        {
            SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs
            {
                UserToken = token,
                SocketFlags = SocketFlags.None
            };
            token.ServerSaea = readEventArgs;
            token.ServerSocket.KeepAlive();
            token.ServerPoolBuffer = new byte[8 * 1024];
            readEventArgs.SetBuffer(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length);
            readEventArgs.Completed += IO_CompletedServer;

            if (token.ServerSocket.ReceiveAsync(readEventArgs) == false)
            {
                ProcessReceiveServer(readEventArgs);
            }
        }
        private void IO_CompletedServer(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceiveServer(e);
                    break;
                default:
                    Logger.Instance.Error(e.LastOperation.ToString());
                    break;
            }
        }
        private async void ProcessReceiveServer(SocketAsyncEventArgs e)
        {
            ProxyClientUserToken token = (ProxyClientUserToken)e.UserToken;
            try
            {
                if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
                {
                    CloseClientSocket(e);
                    return;
                }

                int length = e.BytesTransferred;
                await token.ClientSocket.SendAsync(e.Buffer.AsMemory(0, length), SocketFlags.None);

                if (token.ServerSocket.Available > 0)
                {
                    while (token.ServerSocket.Available > 0)
                    {
                        length = await token.ServerSocket.ReceiveAsync(e.Buffer.AsMemory(0), SocketFlags.None);
                        if (length == 0)
                        {
                            CloseClientSocket(e);
                            return;
                        }

                        await token.ClientSocket.SendAsync(e.Buffer.AsMemory(0, length), SocketFlags.None);
                    }
                }

                if (token.ServerSocket.ReceiveAsync(e) == false)
                {
                    ProcessReceiveServer(e);
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(e);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="e"></param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            ProxyClientUserToken token = e.UserToken as ProxyClientUserToken;
            CloseClientSocket(token);
        }
        private void CloseClientSocket(ProxyClientUserToken token)
        {
            token.ClientSocket?.SafeClose();
            token.ServerSocket?.SafeClose();
            token.PoolBuffer = null;
            token.ServerPoolBuffer = null;
            token.Saea?.Dispose();
            token.ServerSaea?.Dispose();
            GC.Collect();
        }
        #endregion

        #region UDP
        /// <summary>
        /// 连接socks5服务
        /// </summary>
        /// <param name="udpConnection"></param>
        /// <param name="remoteAddress"></param>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <param name="options"></param>
        /// <param name="optionsLen"></param>
        public void ConnectServer(UdpConnection udpConnection, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {

            //缓存目标地址
            udpConnection.RemoteAddress = new byte[(int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH];
            Marshal.Copy(remoteAddress, udpConnection.RemoteAddress, 0, (int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH);
            //缓存udp连接信息
            udpConnection.Options = new byte[optionsLen];
            Marshal.Copy(options, udpConnection.Options, 0, optionsLen);
            //缓存一些其它信息，方便回复数据使用
            AddressFamily addressFamily = (AddressFamily)Marshal.ReadByte(remoteAddress);
            if (addressFamily == AddressFamily.InterNetwork)
            {
                udpConnection.AddressLength = 4;
                udpConnection.AddressType = 0x01;
                udpConnection.AddressOffset = 4;
            }
            else
            {
                udpConnection.AddressLength = 16;
                udpConnection.AddressType = 0x04;
                udpConnection.AddressOffset = 8;
            }

            udpConnection.Socket = CreateConnection(remoteAddress, Socks5EnumRequestCommand.UdpAssociate, out IPEndPoint ServerEP);


            if (udpConnection.Socket != null)
            {
                udpConnection.ServerEP = ServerEP;
                udpConnection.UdpSocket = CreateUdp(udpConnection.ServerEP);
                SendToUdp(udpConnection, buf, len);
                ReceiveUdp(udpConnection);
            }
            else
            {
                udpConnection.SocksFail = true;
            }

        }
        /// <summary>
        /// 发送udp数据
        /// </summary>
        /// <param name="udpConnection"></param>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        public void SendToUdp(UdpConnection udpConnection, nint buf, int len)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(6 + udpConnection.AddressLength + len);
            try
            {
                int index = 0;
                //rsv
                buffer[index] = 0;
                index++;
                buffer[index] = 0;
                index++;
                //flag
                buffer[index] = 0;
                index++;
                //addrtype
                buffer[index] = udpConnection.AddressType;
                index++;

                //addr
                udpConnection.RemoteAddress.AsSpan(udpConnection.AddressOffset, udpConnection.AddressLength).CopyTo(buffer.AsSpan(index));
                index += udpConnection.AddressLength;

                //port
                buffer[index] = udpConnection.RemoteAddress[2];
                index++;
                buffer[index] = udpConnection.RemoteAddress[3];
                index++;

                int headLength = index;

                //data
                Marshal.Copy(buf, buffer, index, len);
                index += len;

                udpConnection.UdpSocket.SendTo(buffer.AsSpan(0, index), udpConnection.ServerEP);
            }
            catch (Exception)
            {
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        /// <summary>
        /// 接收udp数据
        /// </summary>
        /// <param name="udpConnection"></param>
        private void ReceiveUdp(UdpConnection udpConnection)
        {
            udpConnection.UdpBuffer = new byte[65535];
            udpConnection.UdpSocket.BeginReceiveFrom(udpConnection.UdpBuffer, 0, udpConnection.UdpBuffer.Length, SocketFlags.None, ref udpConnection.TempEP, UdpCallback, udpConnection);
        }
        /// <summary>
        /// 接收udp callback
        /// </summary>
        /// <param name="result"></param>
        private unsafe void UdpCallback(IAsyncResult result)
        {
            try
            {
                UdpConnection udpConnection = result.AsyncState as UdpConnection;
                int length = udpConnection.UdpSocket.EndReceiveFrom(result, ref udpConnection.TempEP);

                Memory<byte> data = Socks5Parser.GetUdpData(udpConnection.UdpBuffer.AsMemory(0, length));

                fixed (void* p = &data.Span[0])
                {
                    fixed (void* pAddr = udpConnection.RemoteAddress)
                    {
                        fixed (void* pOptions = udpConnection.Options)
                        {
                            NFAPI.nf_udpPostReceive(udpConnection.Id, (nint)pAddr, (IntPtr)p, data.Length, (nint)pOptions);
                        }
                    }
                }

                udpConnection.UdpSocket.BeginReceiveFrom(udpConnection.UdpBuffer, 0, udpConnection.UdpBuffer.Length, SocketFlags.None, ref udpConnection.TempEP, UdpCallback, udpConnection);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex + "");
            }
        }
        /// <summary>
        /// 创建一个udp socket
        /// </summary>
        /// <param name="serverEP"></param>
        /// <returns></returns>
        public Socket CreateUdp(IPEndPoint serverEP)
        {
            Socket socket = new Socket(serverEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.WindowsUdpBug();
            return socket;
        }

        /// <summary>
        /// 缓存一个连接对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="udpConnection"></param>
        /// <returns></returns>
        public bool AddConnection(ulong id, UdpConnection udpConnection)
        {
            return udpConnections.TryAdd(id, udpConnection);
        }
        /// <summary>
        /// 删除连接对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveConnection(ulong id)
        {
            return udpConnections.TryRemove(id, out _);
        }
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="udpConnection"></param>
        /// <returns></returns>
        public bool GetConnection(ulong id, out UdpConnection udpConnection)
        {
            return udpConnections.TryGetValue(id, out udpConnection);
        }

        #endregion

        #region socks5
        private Socket CreateConnection(nint remoteAddress, Socks5EnumRequestCommand command, out IPEndPoint serverEP)
        {
            serverEP = null;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                if (proxyConfig.IPAddress == null)
                {
                    proxyConfig.Parse();
                }
                IPEndPoint socksEP = new IPEndPoint(proxyConfig.IPAddress, proxyConfig.Proxy.Port);
                Socket socket = new Socket(socksEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(socksEP);
                socket.KeepAlive();

                //请求
                if (Request(socket, buffer, out byte authType) == false)
                {
                    return null;
                }
                //密码认证
                if (authType == 0x02 && Password(socket, buffer) == false)
                {
                    return null;
                }

                if (Command(socket, buffer, remoteAddress, command, out serverEP) == false)
                {
                    return null;
                }
                return socket;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex + "");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            return null;
        }
        private bool Request(Socket socket, byte[] buffer, out byte res)
        {
            res = 0;

            //请求，支持 【无认证】【密码认证】
            buffer[0] = 0x05;
            buffer[1] = 0x02;
            buffer[2] = 0x00;
            buffer[3] = 0x02;
            socket.Send(buffer.AsMemory(0, 4).Span, SocketFlags.None);

            //请求结果
            int length = socket.Receive(buffer, SocketFlags.None);
            Memory<byte> memory = buffer.AsMemory(0, length);
            Span<byte> span = memory.Span;
            //返回数据不符合要求
            if (length < 2 || span[0] != 0x05 || (span[1] != 0x00 && span[1] != 0x02))
            {
                return false;
            }
            res = span[1];
            return true;
        }
        private bool Password(Socket socket, byte[] buffer)
        {
            if (proxyConfig.UserName.Length == 0)
            {
                return false;
            }

            int index = 0;
            buffer[index] = 0x05;

            index++;
            //账号
            buffer[index] = (byte)proxyConfig.UserName.Length;
            index++;
            proxyConfig.UserName.CopyTo(buffer.AsMemory(index));
            index += proxyConfig.Proxy.UserName.Length;
            //密码
            buffer[index] = (byte)proxyConfig.Password.Length;
            index++;
            proxyConfig.Password.CopyTo(buffer.AsMemory(index));
            index += proxyConfig.Proxy.Password.Length;

            socket.Send(buffer.AsMemory(0, index).Span, SocketFlags.None);

            int length = socket.Receive(buffer, SocketFlags.None);
            Memory<byte> memory = buffer.AsMemory(0, length);
            Span<byte> span = memory.Span;

            return length >= 2 && span[1] == 0x00;
        }
        private bool Command(Socket socket, byte[] buffer, nint remoteAddress, Socks5EnumRequestCommand command, out IPEndPoint serverEP)
        {
            serverEP = null;
            AddressFamily addrFamily = (AddressFamily)Marshal.ReadByte(remoteAddress, 0);

            int index = 0;
            buffer[index] = 0x05;
            index++;
            buffer[index] = (byte)command; //command
            index++;
            buffer[index] = 0x00; //rsv
            index++;
            index++;//addr type

            if (addrFamily == AddressFamily.InterNetwork)
            {
                buffer[index - 1] = 0x01;
                Marshal.Copy(remoteAddress + 4, buffer, index, 4);
                index += 4;
            }
            else if (addrFamily == AddressFamily.InterNetworkV6)
            {
                buffer[index - 1] = 0x04;
                Marshal.Copy(remoteAddress + 8, buffer, index, 16);
                index += 16;
            }
            Marshal.Copy(remoteAddress + 2, buffer, index, 2);
            index += 2;

            socket.Send(buffer.AsMemory(0, index).Span, SocketFlags.None);

            int length = socket.Receive(buffer, SocketFlags.None);
            Memory<byte> memory = buffer.AsMemory(0, length);
            Span<byte> span = memory.Span;
            if (length < 6 || span[1] != 0x00)
            {
                return false;
            }

            index = 3;
            IPAddress ip = IPAddress.Any;
            switch ((Socks5EnumAddressType)span[index])
            {
                case Socks5EnumAddressType.IPV4:
                    {
                        ip = new IPAddress(span.Slice(index + 1, 4));
                        index += 1 + 4;
                    }
                    break;
                case Socks5EnumAddressType.Domain:
                    {
                        ip = NetworkHelper.GetDomainIp(Encoding.UTF8.GetString(span.Slice(index + 2, span[index + 1])));
                        index += 2 + span[index + 1];
                    }
                    break;
                case Socks5EnumAddressType.IPV6:
                    {
                        ip = new IPAddress(span.Slice(index + 1, 16));
                        index += 1 + 16;
                    }
                    break;
                default:
                    break;
            }
            if (ip.Equals(IPAddress.Any))
            {
                ip = proxyConfig.IPAddress;
            }
            serverEP = new IPEndPoint(ip, BinaryPrimitives.ReadUInt16BigEndian(span.Slice(index, 2)));
            return true;
        }
        #endregion
    }

    public sealed class UdpConnection
    {
        /// <summary>
        /// 连接id
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public byte[] RemoteAddress { get; set; }
        /// <summary>
        /// ip数据偏移
        /// </summary>
        public byte AddressOffset { get; set; }
        /// <summary>
        /// 目标地址长度
        /// </summary>
        public byte AddressLength { get; set; }
        /// <summary>
        /// 目标地址socks类型
        /// </summary>
        public byte AddressType { get; set; }

        /// <summary>
        /// 连接选项
        /// </summary>
        public byte[] Options { get; set; }

        /// <summary>
        /// 是否代理udp
        /// </summary>
        public bool UDP { get; set; }
        /// <summary>
        /// 是否代理dns
        /// </summary>
        public bool DNS { get; set; }
        /// <summary>
        /// 是否服务端连接事变
        /// </summary>
        public bool SocksFail { get; set; }

        /// <summary>
        /// TCP
        /// </summary>
        public bool Connected => Socket != null && Socket.Connected;
        public Socket Socket { get; set; }

        /// <summary>
        /// UDP
        /// </summary>
        public Socket UdpSocket { get; set; }
        public byte[] UdpBuffer { get; set; }
        public EndPoint TempEP = new IPEndPoint(IPAddress.Any, 0);

        /// <summary>
        /// 服务器地址，连接服务器后，通过socks获得数据转发地址
        /// </summary>
        public IPEndPoint ServerEP { get; set; }
    }

    internal sealed class ProxyClientUserToken
    {
        /// <summary>
        /// 客户端socket，进程劫持那边的
        /// </summary>
        public Socket ClientSocket { get; set; }
        public SocketAsyncEventArgs Saea { get; set; }
        public byte[] PoolBuffer { get; set; }

        /// <summary>
        /// 服务端socket。socks5服务那边的
        /// </summary>
        public Socket ServerSocket { get; set; }
        public byte[] ServerPoolBuffer { get; set; }
        public SocketAsyncEventArgs ServerSaea { get; set; }
        public byte[] Remote { get; set; }
    }
}
