using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using smash.plugins.proxy;
using common.libs.socks5;

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

            udpConnections.TryAdd(id, new UdpConnection { Id = id, DNS = options.Options.DNS });
        }
        public void udpSend(ulong id, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            //是否有连接对象
            if (udpConnections.TryGetValue(id, out UdpConnection udpConnection) == false)
            {
                NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                return;
            }

            //0 1 ip类型，2 3 端口，剩余的是根据ip类型的不同长度的ip数据
            byte port0 = Marshal.ReadByte(remoteAddress, 2);
            byte port1 = Marshal.ReadByte(remoteAddress, 3);
            if (port0 == 0 && port1 == 53 && udpConnection.DNS == false)
            {
                NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                return;
            }

            //构建socks5连接
            if (udpConnection.Socket == null)
            {
                udpConnection.RemoteAddress = remoteAddress;
                udpConnection.Options = options;
                udpConnection.Socket = hijackServer.CreateConnection(remoteAddress, Socks5EnumRequestCommand.UdpAssociate, out IPEndPoint ServerEP);
                udpConnection.ServerEP = ServerEP;

                udpConnection.UdpSocket = hijackServer.CreateUdp(udpConnection.ServerEP);
                SendToUdp(udpConnection, buf, len);
                ReceiveUdp(udpConnection);
                return;
            }
            SendToUdp(udpConnection, buf, len);
        }
        private unsafe void SendToUdp(UdpConnection udpConnection, nint buf, int len)
        {
            udpConnection.UdpSocket.SendTo(new Span<byte>(buf.ToPointer(), len), udpConnection.ServerEP);
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

                fixed (void* p = &data.Span[0])
                {
                    NFAPI.nf_udpPostReceive(udpConnection.Id, udpConnection.RemoteAddress, (IntPtr)p, length, udpConnection.Options);
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
        public nint RemoteAddress { get; set; }
        public nint Options { get; set; }

        public bool DNS { get; set; }

        public Socket Socket { get; set; }
        public Socket UdpSocket { get; set; }
        public byte[] UdpBuffer { get; set; }
        public EndPoint TempEP;

        public IPEndPoint ServerEP { get; set; }
    }

}
