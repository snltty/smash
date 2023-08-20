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

namespace smash.plugins.hijack
{
    public class HijackServer
    {
        private readonly ProxyConfig proxyConfig;
        public HijackServer(ProxyConfig proxyConfig)
        {
            this.proxyConfig = proxyConfig;
        }

        public int Start()
        {
            return 0;
        }

        public Socket CreateUdp(IPEndPoint serverEP)
        {
            Socket socket = new Socket(serverEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.WindowsUdpBug();
            return socket;
        }
        public Socket CreateConnection(nint remoteAddress, Socks5EnumRequestCommand command, out IPEndPoint serverEP)
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
                Marshal.Copy(remoteAddress + index, buffer, index, 4);
                index += 4;
            }
            else if (addrFamily == AddressFamily.InterNetworkV6)
            {
                buffer[index - 1] = 0x04;
                Marshal.Copy(remoteAddress + index, buffer, index, 16);
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
    }
}
