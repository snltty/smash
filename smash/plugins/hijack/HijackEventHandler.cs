using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using smash.plugins.proxy;

namespace smash.plugins.hijack
{
    public sealed class HijackEventHandler : NF_EventHandler
    {
        private readonly HijackConfig hijackConfig;
        private readonly uint currentProcessId = 0;
        private readonly ConcurrentDictionary<ulong, UdpConnection> udpConnections = new ConcurrentDictionary<ulong, UdpConnection>();

        public HijackEventHandler(HijackConfig hijackConfig)
        {
            this.hijackConfig = hijackConfig;
            currentProcessId = (uint)Process.GetCurrentProcess().Id;
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
        #endregion

        public void tcpClosed(ulong id, NF_TCP_CONN_INFO pConnInfo)
        {
            //删除tcp对象缓存
        }
        public void tcpConnectRequest(ulong id, ref NF_TCP_CONN_INFO pConnInfo)
        {
            if (hijackConfig.FilterTCP == false)
            {
                //NFAPI.nf_tcpDisableFiltering(pConnInfo.processId);
                // return;
            }
            if (checkProcess(pConnInfo.processId, out string processName) == false)
            {
                //NFAPI.nf_tcpDisableFiltering(pConnInfo.processId);
                // return;
            }


            //更改目标地址到tcp连接
            RemoteIPEndPint localEp = convertAddress(pConnInfo.localAddress);
            RemoteIPEndPint remoteEp = convertAddress(pConnInfo.remoteAddress);
            //判断是否dns
            if (remoteEp.Port == 53 && hijackConfig.FilterDNS == false)
            {
                //return;
            }


            Debug.WriteLine($"tcp request {processName}->{localEp.IPEndPoint}->{remoteEp.IPEndPoint}");
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
        public void udpCreated(ulong id, NF_UDP_CONN_INFO pConnInfo)
        {
            if (hijackConfig.FilterUDP == false)
            {
                //NFAPI.nf_udpDisableFiltering(pConnInfo.processId);
                //return;
            }
            if (checkProcess(pConnInfo.processId, out string processName) == false)
            {
                // NFAPI.nf_udpDisableFiltering(pConnInfo.processId);
                //return;
            }

            udpConnections.TryAdd(id, new UdpConnection { Id = id });
        }
        public void udpClosed(ulong id, NF_UDP_CONN_INFO pConnInfo)
        {
            udpConnections.TryRemove(id, out _);
            //删除udp对象缓存
        }
        public void udpSend(ulong id, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            //是否有连接对象
            if (udpConnections.TryGetValue(id, out UdpConnection udpConnection) == false)
            {
                //NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                //return;
            }

            byte[] remoteAddressBuf = new byte[(int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH];
            Marshal.Copy(remoteAddress, remoteAddressBuf, 0, (int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH);
            RemoteIPEndPint remoteEp = convertAddress(remoteAddressBuf);

            //判断是否dns
            if (remoteEp.Port == 53 && hijackConfig.FilterDNS == false)
            {
                //NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
                //return;
            }

            udpConnection.Id = id;
            udpConnection.Options = options;

            //构建socks5连接
            NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);

            // Debug.WriteLine($"udp send xxx->{remoteEp.IPEndPoint}");
        }


        private bool checkProcess(uint processId, out string processName)
        {
            processName = string.Empty;
            if (currentProcessId == processId)
            {
                return false;
            }

            processName = NFAPI.nf_getProcessName(processId);
            if (currentProcessId == processId || checkProcessName(processName) == false)
            {
                return false;
            }

            return true;
        }
        private bool checkProcessName(string path)
        {
            for (int i = 0; i < hijackConfig.CurrentProcesss.Length; i++)
            {
                if (hijackConfig.CurrentProcesss[i].Length > path.Length) break;

                var pathSpan = path.AsSpan();
                var nameSpan = hijackConfig.CurrentProcesss[i].AsSpan();
                try
                {
                    if (pathSpan.Slice(pathSpan.Length - nameSpan.Length, nameSpan.Length).SequenceEqual(nameSpan))
                    {
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
        private RemoteIPEndPint convertAddress(byte[] buf)
        {
            if (buf == null)
            {
                return null;
            }

            Span<byte> spsn = buf.AsSpan();
            if ((AddressFamily)buf[0] == AddressFamily.InterNetwork)
            {
                ushort port = BinaryPrimitives.ReadUInt16BigEndian(spsn.Slice(2, 2));
                return new RemoteIPEndPint
                {
                    IPEndPoint = new IPEndPoint(new IPAddress(spsn.Slice(4, 4)), port),
                    Port = port,
                    IP = BinaryPrimitives.ReadUInt16BigEndian(spsn.Slice(4, 4))
                };
            }
            else if ((AddressFamily)buf[0] == AddressFamily.InterNetworkV6)
            {
                ushort port = BinaryPrimitives.ReadUInt16BigEndian(spsn.Slice(2, 2));
                return new RemoteIPEndPint
                {
                    IPEndPoint = new IPEndPoint(new IPAddress(spsn.Slice(4, 16)), port),
                    Port = port,
                    IP = 0
                };
            }
            return null;
        }

    }


    public sealed class RemoteIPEndPint
    {
        public IPEndPoint IPEndPoint { get; set; }
        public uint IP { get; set; }
        public ushort Port { get; set; }
    }
    public sealed class TcpConnection
    {

    }
    public sealed class UdpConnection
    {
        public ulong Id { get; set; }
        public nint Options { get; set; }
    }

}
