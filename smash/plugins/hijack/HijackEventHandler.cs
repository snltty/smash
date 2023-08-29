using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using smash.plugins.proxy;
using System.Buffers.Binary;

namespace smash.plugins.hijack
{
    public sealed class HijackEventHandler : NF_EventHandler
    {
        private readonly HijackConfig hijackConfig;
        private readonly ProxyConfig proxyConfig;
        private readonly HijackServer hijackServer;
        private readonly uint currentProcessId = 0;
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
        public void tcpConnectRequest(ulong id, ref NF_TCP_CONN_INFO pConnInfo)
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
            //缓存以下对应关系，等下服务端收到连接后，可以知道需要连哪个服务
            ushort localPort = (ushort)((pConnInfo.localAddress[2] << 8 & 0xFF00) | pConnInfo.localAddress[3]);
            byte[] remote = new byte[pConnInfo.remoteAddress.Length];
            pConnInfo.remoteAddress.AsSpan().CopyTo(remote);
            hijackServer.CacheEndPoint(localPort, remote);

            //强行修改为ipv4
            pConnInfo.remoteAddress[0] = (byte)AddressFamily.InterNetwork;
            //更改目标地址到劫持服务器
            ipaddress.AsSpan().CopyTo(pConnInfo.remoteAddress.AsSpan(4));

            pConnInfo.remoteAddress[2] = (byte)(port >> 8 & 0xff);
            pConnInfo.remoteAddress[3] = (byte)(port & 0xff);
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
            hijackServer.RemoveConnection(id);
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

            hijackServer.AddConnection(id, new UdpConnection { Id = id, DNS = options.Options.DNS, UDP = options.Options.UDP });
        }
        public unsafe void udpSend(ulong id, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            //没连接对象，那在udpCreated那里就已经被阻止了，直接发送数据即可
            if (hijackServer.GetConnection(id, out UdpConnection udpConnection) == false)
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
                NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                return;
            }
            //连接代理服务器
            if (udpConnection.Connected == false)
            {
                hijackServer.ConnectServer(udpConnection, remoteAddress, buf, len, options, optionsLen);
                if (udpConnection.SocksFail)
                {
                    //服务器连接失败，直接提交数据
                    NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                }
                return;
            }

            hijackServer.SendToUdp(udpConnection, buf, len);
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
            return checkProcessName(processName, out options);
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
}
