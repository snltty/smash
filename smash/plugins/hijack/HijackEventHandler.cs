using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using smash.plugins.proxy;
using common.libs.socks5;
using static System.Runtime.InteropServices.JavaScript.JSType;
using common.libs.extends;
using System.Buffers.Binary;
using System.Buffers;
using static System.Windows.Forms.Design.AxImporter;
using System.Drawing;

namespace smash.plugins.hijack
{
    public sealed class HijackEventHandler : NF_EventHandler
    {
        private readonly HijackConfig hijackConfig;
        private readonly ProxyConfig proxyConfig;
        private readonly HijackServer hijackServer;
        private readonly uint currentProcessId = 0;
        private readonly ConcurrentDictionary<ulong, UdpConnection> udpConnections = new ConcurrentDictionary<ulong, UdpConnection>();
        private readonly byte[] ipaddress = IPAddress.Loopback.GetAddressBytes();
        private ushort port;

        public HijackEventHandler(HijackConfig hijackConfig, ProxyConfig proxyConfig, HijackServer hijackServer)
        {
            this.hijackConfig = hijackConfig;
            this.proxyConfig = proxyConfig;
            this.hijackServer = hijackServer;
            currentProcessId = (uint)Process.GetCurrentProcess().Id;

            port = (ushort)hijackServer.Start();
        }

        #region tcp无需处理
        public void tcpCanReceive(ulong id)
        {
        }
        public void tcpCanSend(ulong id)
        {
        }
        public void tcpConnected(ulong id, NF_TCP_CONN_INFO pConnInfo)
        {
        }
        public void tcpSend(ulong id, nint buf, int len)
        {
            NFAPI.nf_tcpPostSend(id, buf, len);
        }
        public void tcpReceive(ulong id, nint buf, int len)
        {
            NFAPI.nf_tcpPostReceive(id, buf, len);
        }
        public void tcpClosed(ulong id, NF_TCP_CONN_INFO pConnInfo)
        {
            //删除tcp对象缓存
        }
        #endregion
        public unsafe void tcpConnectRequest(ulong id, ref NF_TCP_CONN_INFO pConnInfo)
        {
            if (checkProcess(pConnInfo.processId, out string processName, out ProcessParseInfo options) == false)
            {
                NFAPI.nf_tcpDisableFiltering(pConnInfo.processId);
                return;
            }
            if (options.Options.TCP == false)
            {
                NFAPI.nf_tcpDisableFiltering(pConnInfo.processId);
                return;
            }


            //更改目标地址到劫持服务器
            if (proxyConfig.IPAddress != null)
            {
                fixed (void* p = &ipaddress[0])
                {
                    Marshal.Copy((IntPtr)p, pConnInfo.remoteAddress, 4, ipaddress.Length);
                }
                fixed (ushort* p = &port)
                {
                    byte* pp = (byte*)p;
                    pConnInfo.remoteAddress[2] = *(pp + 1);
                    pConnInfo.remoteAddress[3] = *(pp);
                }
            }
        }

        #region udp无需处理
        public void udpCanReceive(ulong id)
        {
        }
        public void udpCanSend(ulong id)
        {
        }
        public void udpConnectRequest(ulong id, ref NF_UDP_CONN_REQUEST pConnReq)
        {
        }
        public void threadEnd()
        {
        }
        public void threadStart()
        {
        }
        public void udpReceive(ulong id, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            NFAPI.nf_udpPostReceive(id, remoteAddress, buf, len, options);
        }
        #endregion
        public void udpClosed(ulong id, NF_UDP_CONN_INFO pConnInfo)
        {
            udpConnections.TryRemove(id, out _);
        }
        public void udpCreated(ulong id, NF_UDP_CONN_INFO pConnInfo)
        {
            //不是需要代理的进程
            if (checkProcess(pConnInfo.processId, out string processName, out ProcessParseInfo options) == false)
            {
                NFAPI.nf_udpDisableFiltering(pConnInfo.processId);
                return;
            }
            //不代理UDP 且 不代理DNS
            if (options.Options.UDP == false && options.Options.DNS == false)
            {
                NFAPI.nf_udpDisableFiltering(pConnInfo.processId);
                return;
            }

            udpConnections.TryAdd(id, new UdpConnection { Id = id, DNS = options.Options.DNS, UDP = options.Options.UDP });
        }
        public unsafe void udpSend(ulong id, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            //没连接对象，那在udpCreated那里就已经被阻止了，直接发送数据即可
            if (udpConnections.TryGetValue(id, out UdpConnection udpConnection) == false)
            {
                NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                return;
            }
            //之前连接过服务器，失败了，直接提交数据
            if (udpConnection.SocksFail)
            {
                NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                return;
            }

            //大端端口，低地址存着高字节，那就把低地址的数据左移到高字节，把高地址的留在低字节，就成了小端
            byte* p = (byte*)remoteAddress;
            ushort port = (ushort)((*(p + 2) << 8 & 0xFF00) | *(p + 3));
            //是DNS但不代理， 或不是DNS也不代理
            if ((port == 53 && udpConnection.DNS == false) || (port != 53 && udpConnection.UDP == false))
            {
                if (udpConnection.DNS == false)
                {
                    NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                    return;
                }
            }

            //连接代理服务器
            if (udpConnection.Connected == false)
            {
                ConnectServer(udpConnection, remoteAddress, buf, len, options, optionsLen);
                if (udpConnection.SocksFail)
                {
                    //服务器连接失败，直接提交数据
                    NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                }
                return;
            }

            SendToUdp(udpConnection, buf, len);
        }
        private void ConnectServer(UdpConnection udpConnection, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            //缓存目标地址
            udpConnection.RemoteAddress = new byte[(int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH];
            Marshal.Copy(remoteAddress, udpConnection.RemoteAddress, 0, (int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH);
            //缓存udp连接信息
            udpConnection.Options = new byte[optionsLen];
            Marshal.Copy(options, udpConnection.Options, 0, optionsLen);
            //缓存一些其它信息，方便回复数据使用
            udpConnection.AddressFamily = (AddressFamily)Marshal.ReadByte(remoteAddress);
            if (udpConnection.AddressFamily == AddressFamily.InterNetwork)
            {
                udpConnection.AddressLength = 4;
                udpConnection.AddressType = 0x01;
                udpConnection.AddressIndex = 4;
            }
            else
            {
                udpConnection.AddressLength = 16;
                udpConnection.AddressType = 0x04;
                udpConnection.AddressIndex = 8;
            }

            udpConnection.Socket = hijackServer.CreateConnection(remoteAddress, Socks5EnumRequestCommand.UdpAssociate, out IPEndPoint ServerEP);
            if (udpConnection.Socket != null)
            {
                udpConnection.ServerEP = ServerEP;
                udpConnection.UdpSocket = hijackServer.CreateUdp(udpConnection.ServerEP);
                SendToUdp(udpConnection, buf, len);
                ReceiveUdp(udpConnection);
            }
            else
            {
                udpConnection.SocksFail = true;
            }
        }
        private unsafe void SendToUdp(UdpConnection udpConnection, nint buf, int len)
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
                udpConnection.RemoteAddress.AsSpan(udpConnection.AddressIndex, udpConnection.AddressLength).CopyTo(buffer.AsSpan(index));
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

                Memory<byte> remote = Socks5Parser.GetRemoteEndPoint(buffer.AsMemory(), out Socks5EnumAddressType addressType, out ushort port, out int index1);
                Debug.WriteLine($"udp proxy send to remote {new IPEndPoint(new IPAddress(remote.Span), port)} : {string.Join(",", buffer.AsMemory(0, headLength).ToArray())}");

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
        private void ReceiveUdp(UdpConnection udpConnection)
        {
            udpConnection.UdpBuffer = new byte[65535];
            udpConnection.UdpSocket.BeginReceiveFrom(udpConnection.UdpBuffer, 0, udpConnection.UdpBuffer.Length, SocketFlags.None, ref udpConnection.TempEP, UdpCallback, udpConnection);
        }
        private unsafe void UdpCallback(IAsyncResult result)
        {
            try
            {
                UdpConnection udpConnection = result.AsyncState as UdpConnection;
                int length = udpConnection.UdpSocket.EndReceiveFrom(result, ref udpConnection.TempEP);

                Memory<byte> memory = udpConnection.UdpBuffer.AsMemory(0, length);
                Memory<byte> data = Socks5Parser.GetUdpData(memory);

                Debug.WriteLine($"udp {(udpConnection.TempEP as IPEndPoint).ToString()} receive :{string.Join(",", data.ToArray())}");

                fixed (void* p = &data.Span[0])
                {
                    fixed (void* pAddr = &udpConnection.RemoteAddress[0])
                    {
                        fixed (void* pOptions = &udpConnection.Options[0])
                        {
                            NFAPI.nf_udpPostReceive(udpConnection.Id, (nint)pAddr, (IntPtr)p, length, (nint)pOptions);
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

        private bool checkProcess(uint processId, out string processName, out ProcessParseInfo options)
        {
            processName = string.Empty;
            options = null;
            if (currentProcessId == processId)
            {
                return false;
            }

            processName = NFAPI.nf_getProcessName(processId);
            if (currentProcessId == processId || checkProcessName(processName, out options) == false)
            {
                return false;
            }

            return true;
        }
        private bool checkProcessName(string path, out ProcessParseInfo options)
        {
            options = null;
            for (int i = 0; i < hijackConfig.CurrentProcesss.Length; i++)
            {
                if (hijackConfig.CurrentProcesss[i].Name.Length > path.Length) break;

                var pathSpan = path.AsSpan();
                var nameSpan = hijackConfig.CurrentProcesss[i].Name.AsSpan();
                try
                {
                    if (pathSpan.Slice(pathSpan.Length - nameSpan.Length, nameSpan.Length).SequenceEqual(nameSpan))
                    {
                        options = hijackConfig.CurrentProcesss[i];
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex + "");
                    Debug.WriteLine($"{hijackConfig.CurrentProcesss[i]}->{path}->{hijackConfig.CurrentProcesss[i]}");
                }
            }
            return false;
        }
    }

    public sealed class UdpConnection
    {
        public ulong Id { get; set; }
        public byte[] RemoteAddress { get; set; }
        public AddressFamily AddressFamily { get; set; }
        public byte AddressLength { get; set; }
        public byte AddressType { get; set; }
        public byte AddressIndex { get; set; }
        public byte[] Options { get; set; }

        public bool UDP { get; set; }
        public bool DNS { get; set; }

        public bool SocksFail { get; set; }

        public bool Connected => Socket != null && Socket.Connected;
        public Socket Socket { get; set; }
        public Socket UdpSocket { get; set; }
        public byte[] UdpBuffer { get; set; }
        public EndPoint TempEP = new IPEndPoint(IPAddress.Any, 0);

        public IPEndPoint ServerEP { get; set; }
    }

}
