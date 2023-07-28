using common.libs;
using common.libs.extends;
using System;
using System.Buffers;
using System.Text;

namespace smash.proxy
{
    public sealed class ProxyInfo
    {
        static Memory<byte> HeaderEnd = Encoding.UTF8.GetBytes("\r\n\r\n");

        /// <summary>
        /// 地址类型
        /// </summary>
        public Socks5EnumAddressType AddressType { get; set; } = Socks5EnumAddressType.IPV4;
        public Socks5EnumRequestCommand Command { get; set; } = Socks5EnumRequestCommand.Connect;

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


        public bool ValidateKey(Memory<byte> data, Memory<byte> key)
        {
            return data.Length >= key.Length && data.Span.Slice(0, key.Length).SequenceEqual(key.Span);
        }

        public byte[] PackConnect(Memory<byte> httpHeader, out int length)
        {
            int protocolLength =
                +1 // 0000 0000 address type + command
                + 1 + TargetAddress.Length + 2;// target length

            //协议数据长度 + 描述长度的4字节
            int contentLength = protocolLength + 4;
            byte[] contentLengthStr = contentLength.ToString().ToBytes();

            length =
               +httpHeader.Length //http header
               + contentLengthStr.Length + HeaderEnd.Length //content-length 数值长度 + \r\n\r\n
               + contentLength;


            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory(0, length);
            var span = memory.Span;
            int index = 0;

            //http请求头
            httpHeader.CopyTo(memory.Slice(index));
            index += httpHeader.Length;

            contentLengthStr.CopyTo(memory.Slice(index));
            index += contentLengthStr.Length;

            HeaderEnd.CopyTo(memory.Slice(index));
            index += HeaderEnd.Length;

            //协议内容
            bytes[index] = (byte)((byte)AddressType << 4 | (byte)Command);
            index += 1;

            bytes[index] = (byte)TargetAddress.Length;
            index += 1;

            TargetAddress.CopyTo(memory.Slice(index));
            index += TargetAddress.Length;
            TargetPort.ToBytes(memory.Slice(index));
            index += 2;

            //协议长度
            protocolLength.ToBytes(memory.Slice(index));
            index += 4;

            return bytes;
        }
        public bool UnPackConnect(Memory<byte> bytes)
        {
            var span = bytes.Span;

            int protocolLength = span.Slice(span.Length - 4, 4).ToInt32();
            bytes = bytes.Slice(span.Length - protocolLength - 4, protocolLength);
            span = bytes.Span;
            int index = 0;

            AddressType = (Socks5EnumAddressType)(span[index] >> 4);
            Command = (Socks5EnumRequestCommand)(span[index] & 0b0000_1111);
            index += 1;


            byte targetepLength = span[index];
            index += 1;
            TargetAddress = bytes.Slice(index, targetepLength);
            index += targetepLength;
            TargetPort = span.Slice(index, 2).ToUInt16();
            index += 2;

            return true;
        }

        public byte[] PackData(Memory<byte> httpHeader, out int length)
        {

            byte[] contentLengthStr = Data.Length.ToString().ToBytes();
            length =
              +httpHeader.Length //http header
              + contentLengthStr.Length + HeaderEnd.Length //content-length 数值长度 + \r\n\r\n
              + Data.Length;


            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory(0, length);
            var span = memory.Span;
            int index = 0;


            //http请求头
            httpHeader.CopyTo(memory.Slice(index));
            index += httpHeader.Length;

            contentLengthStr.CopyTo(memory.Slice(index));
            index += contentLengthStr.Length;

            HeaderEnd.CopyTo(memory.Slice(index));
            index += HeaderEnd.Length;

            Data.CopyTo(memory.Slice(index));

            return bytes;
        }
        public static byte[] PackResponse(Memory<byte> httpHeader, Memory<byte> data, out int length)
        {

            byte[] contentLengthStr = data.Length.ToString().ToBytes();
            length =
              +httpHeader.Length //http header
              + contentLengthStr.Length + HeaderEnd.Length //content-length 数值长度 + \r\n\r\n
              + data.Length;


            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory(0, length);
            int index = 0;

            //http请求头
            httpHeader.CopyTo(memory.Slice(index));
            index += httpHeader.Length;

            contentLengthStr.CopyTo(memory.Slice(index));
            index += contentLengthStr.Length;

            HeaderEnd.CopyTo(memory.Slice(index));
            index += HeaderEnd.Length;

            data.CopyTo(memory.Slice(index));

            return bytes;
        }

        public void Return(byte[] data)
        {
            ReturnStatic(data);
        }
        public static void ReturnStatic(byte[] data)
        {
            ArrayPool<byte>.Shared.Return(data);
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
