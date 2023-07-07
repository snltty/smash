using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace smash.libs.hijack
{
    public sealed class HijackEventHandler : NF_EventHandler
    {
        private readonly Config config;
        private readonly uint currentProcessId = 0;
        public HijackEventHandler(Config config)
        {
            this.config = config;
            currentProcessId = (uint)Process.GetCurrentProcess().Id;
        }

        #region tcp无需处理
        public void tcpCanReceive(ulong id)
        {
        }
        public void tcpCanSend(ulong id)
        {
        }
        #endregion
        public void tcpClosed(ulong id, NF_TCP_CONN_INFO pConnInfo)
        {
            //删除tcp对象缓存
        }
        public void tcpConnectRequest(ulong id, ref NF_TCP_CONN_INFO pConnInfo)
        {
            if(currentProcessId == pConnInfo.processId)
            {
                NFAPI.nf_tcpDisableFiltering(pConnInfo.processId);
                return;
            }
            string processName = NFAPI.nf_getProcessName(pConnInfo.processId);

            RemoteIPEndPint localEp = convertAddress(pConnInfo.localAddress);
            RemoteIPEndPint remoteEp = convertAddress(pConnInfo.remoteAddress);
            Debug.WriteLine($"tcpConnectRequest:================================");
            Debug.WriteLine($"{processName}->{localEp.IPEndPoint}->{remoteEp.IPEndPoint}");
            Debug.WriteLine($"tcpConnectRequest:================================");

            return;

            if (config.CurrentProcesss.ContainsKey(processName) == false)
            {
                NFAPI.nf_tcpDisableFiltering(id);
                return;
            }
            //判断进程
            //判断过滤条件
            //创建连接对象
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
        #endregion

        public void udpCreated(ulong id, NF_UDP_CONN_INFO pConnInfo)
        {
            if (currentProcessId == pConnInfo.processId)
            {
                NFAPI.nf_udpDisableFiltering(pConnInfo.processId);
                return;
            }
            string processName = NFAPI.nf_getProcessName(pConnInfo.processId);


            //判断进程
            //判断过滤条件
            //创建udp发送对象
        }
        public void udpClosed(ulong id, NF_UDP_CONN_INFO pConnInfo)
        {
            //删除udp对象缓存
        }
        public void udpSend(ulong id, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            //出站

            //判断是否dns
            //判断是否有连接对象，没有的，直接nf_udpPostSend
            //构建socks5连接
            //发送udp数据
            NFAPI.nf_udpPostSend(id, remoteAddress, buf, len, options);
            //接收数据，通过nf_udpPostReceive(id, (unsigned char*)&target, buffer, length, options);回复

            byte[] remoteAddressBuf = new byte[(int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH];
            Marshal.Copy((IntPtr)remoteAddress, remoteAddressBuf, 0, (int)NF_CONSTS.NF_MAX_ADDRESS_LENGTH);
            RemoteIPEndPint remoteEp = convertAddress(remoteAddressBuf);

           // Debug.WriteLine($"udpSend:+++++++++++++++++++++++++++++++++");
           // Debug.WriteLine($"xxx->{string.Join(",", remoteAddressBuf)}");
           // Debug.WriteLine($"xxx->{remoteEp.IPEndPoint}");
          //  Debug.WriteLine($"udpSend:+++++++++++++++++++++++++++++++++");
        }
        public void udpReceive(ulong id, nint remoteAddress, nint buf, int len, nint options, int optionsLen)
        {
            //入站
            NFAPI.nf_udpPostReceive(id, remoteAddress, buf, len, options);
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

}
