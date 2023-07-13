using common.libs.extends;
using System;
using System.Buffers;
using System.Net;

namespace smash.proxy
{
    public class ProxyInfo
    {
        /// <summary>
        /// 执行到哪一步了
        /// </summary>
        public Socks5EnumStep Step { get; set; } = Socks5EnumStep.Request;
        /// <summary>
        /// 连接类型
        /// </summary>
        public Socks5EnumRequestCommand Command { get; set; } = Socks5EnumRequestCommand.Connect;
        /// <summary>
        /// 地址类型
        /// </summary>
        public Socks5EnumAddressType AddressType { get; set; } = Socks5EnumAddressType.IPV4;
        /// <summary>
        /// buffer size
        /// </summary>
        public EnumBufferSize BufferSize { get; set; } = EnumBufferSize.KB_8;

        public Socks5EnumResponseCommand CommandStatus { get; set; }

        /// <summary>
        /// 请求id
        /// </summary>
        public uint RequestId { get; set; }
        /// <summary>
        /// 来源地址，数据从目标端回来的时候回给谁
        /// </summary>
        public IPEndPoint SourceEP { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public Memory<byte> TargetAddress { get; set; }
        public ushort TargetPort { get; set; }

        /// <summary>
        /// 携带的数据
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public Memory<byte> Data { get; set; }


        public byte[] ToBytes(out int length)
        {
            length = 1 //0000 0000  step + command
                + 1 // 0000 0000 address type + buffer size
                + 1 //CommandResponse
                + 4  // RequestId
                + 1  //source length
                + 1 // target length
                + Data.Length;

            int sepLength = 0;
            if (SourceEP != null)
            {
                sepLength = SourceEP.Address.Length();
                length += sepLength + 2;
            }
            if (TargetAddress.Length > 0)
            {
                length += TargetAddress.Length + 2;
            }

            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory(0, length);
            var span = memory.Span;
            int index = 0;


            bytes[index] = (byte)(((byte)Step) << 4 | (byte)Command);
            index += 1;
            bytes[index] = (byte)(((byte)AddressType << 4) | (byte)BufferSize);
            index += 1;

            bytes[index] = (byte)CommandStatus;
            index += 1;

            RequestId.ToBytes(memory.Slice(index));
            index += 4;

            bytes[index] = (byte)sepLength;
            index += 1;
            if (sepLength > 0)
            {
                SourceEP.Address.TryWriteBytes(span.Slice(index), out _);
                index += sepLength;

                ((ushort)SourceEP.Port).ToBytes(memory.Slice(index));
                index += 2;
            }

            bytes[index] = (byte)TargetAddress.Length;
            index += 1;
            if (TargetAddress.Length > 0)
            {
                TargetAddress.CopyTo(memory.Slice(index));
                index += TargetAddress.Length;
                TargetPort.ToBytes(memory.Slice(index));
                index += 2;
            }


            if (Data.Length > 0)
            {
                Data.CopyTo(memory.Slice(index));
            }
            return bytes;
        }
        public void DeBytes(Memory<byte> bytes)
        {
            var span = bytes.Span;
            int index = 0;

            Step = (EnumProxyStep)(span[index] >> 4);
            Command = (EnumProxyCommand)(span[index] & 0b0000_1111);
            index++;

            AddressType = (EnumProxyAddressType)(span[index] >> 4);
            BufferSize = (EnumBufferSize)(span[index] & 0b0000_1111);
            index += 1;

            CommandStatus = (EnumProxyCommandStatus)span[index];
            index += 1;

            RequestId = span.Slice(index).ToUInt32();
            index += 4;

            byte epLength = span[index];
            index += 1;
            if (epLength > 0)
            {
                IPAddress ip = new IPAddress(span.Slice(index, epLength));
                index += epLength;
                SourceEP = new IPEndPoint(ip, span.Slice(index, 2).ToUInt16());
                index += 2;
            }

            byte targetepLength = span[index];
            index += 1;
            if (targetepLength > 0)
            {
                TargetAddress = bytes.Slice(index, targetepLength);
                index += targetepLength;
                TargetPort = span.Slice(index, 2).ToUInt16();
                index += 2;
            }

            Data = bytes.Slice(index);
        }
        public static ProxyInfo Debytes(Memory<byte> data)
        {
            ProxyInfo info = new ProxyInfo();
            info.DeBytes(data);
            return info;
        }
        public void Return(byte[] data)
        {
            ArrayPool<byte>.Shared.Return(data);
        }

        public static uint GetRequestId(Memory<byte> bytes)
        {
            return bytes.Span.Slice(3).ToUInt32();
        }
    }

    public enum EnumBufferSize : byte
    {
        KB_1 = 0,
        KB_2 = 1,
        KB_4 = 2,
        KB_8 = 3,
        KB_16 = 4,
        KB_32 = 5,
        KB_64 = 6,
        KB_128 = 7,
        KB_256 = 8,
        KB_512 = 9,
        KB_1024 = 10,
    }

    
    /// <summary>
    /// 数据验证结果
    /// </summary>
    [Flags]
    public enum EnumProxyValidateDataResult : byte
    {
        Equal = 1,
        TooShort = 2,
        TooLong = 4,
        Bad = 8,
    }

    /// <summary>
    /// 当前处于socks5协议的哪一步
    /// </summary>
    public enum Socks5EnumStep : byte
    {
        /// <summary>
        /// 第一次请求，处理认证方式
        /// </summary>
        Request = 1,
        /// <summary>
        /// 如果有认证
        /// </summary>
        Auth = 2,
        /// <summary>
        /// 发送命令，CONNECT BIND 还是  UDP ASSOCIATE
        /// </summary>
        Command = 3,
        /// <summary>
        /// 转发
        /// </summary>
        Forward = 4,
        /// <summary>
        /// udp转发
        /// </summary>
        ForwardUdp = 5,

        None = 0
    }

    /// <summary>
    /// socks5的连接地址类型
    /// </summary>
    public enum Socks5EnumAddressType : byte
    {
        IPV4 = 1,
        Domain = 3,
        IPV6 = 4
    }

    /// <summary>
    /// socks5的认证类型
    /// </summary>
    public enum Socks5EnumAuthType : byte
    {
        NoAuth = 0x00,
        GSSAPI = 0x01,
        Password = 0x02,
        IANA = 0x03,
        UnKnow = 0x80,
        NotSupported = 0xff,
    }
    /// <summary>
    /// socks5的认证状态0成功 其它失败
    /// </summary>
    public enum Socks5EnumAuthState : byte
    {
        Success = 0x00,
        UnKnow = 0xff,
    }
    /// <summary>
    /// socks5的请求指令
    /// </summary>
    public enum Socks5EnumRequestCommand : byte
    {
        /// <summary>
        /// 连接上游服务器
        /// </summary>
        Connect = 1,
        /// <summary>
        /// 绑定，客户端会接收来自代理服务器的链接，著名的FTP被动模式
        /// </summary>
        Bind = 2,
        /// <summary>
        /// UDP中继
        /// </summary>
        UdpAssociate = 3
    }
    /// <summary>
    /// socks5的请求的回复数据的指令
    /// </summary>
    public enum Socks5EnumResponseCommand : byte
    {
        /// <summary>
        /// 代理服务器连接目标服务器成功
        /// </summary>
        ConnecSuccess = 0,
        /// <summary>
        /// 代理服务器故障
        /// </summary>
        ServerError = 1,
        /// <summary>
        /// 代理服务器规则集不允许连接
        /// </summary>
        ConnectNotAllow = 2,
        /// <summary>
        /// 网络无法访问
        /// </summary>
        NetworkError = 3,
        /// <summary>
        /// 目标服务器无法访问（主机名无效）
        /// </summary>
        ConnectFail = 4,
        /// <summary>
        /// 连接目标服务器被拒绝
        /// </summary>
        DistReject = 5,
        /// <summary>
        /// TTL已过期
        /// </summary>
        TTLTimeout = 6,
        /// <summary>
        /// 不支持的命令
        /// </summary>
        CommandNotAllow = 7,
        /// <summary>
        /// 不支持的目标服务器地址类型
        /// </summary>
        AddressNotAllow = 8,
        /// <summary>
        /// 未分配
        /// </summary>
        Unknow = 8,
    }


}
