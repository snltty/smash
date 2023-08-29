using common.libs.extends;
using common.libs.socks5;
using System;
using System.Buffers;

namespace smash.proxy.protocol
{
    internal sealed class ProxyInfo
    {
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


        [System.Text.Json.Serialization.JsonIgnore]
        public Memory<byte> ResponseData { get; set; }


        public bool ValidateKey(Memory<byte> data, Memory<byte> key)
        {
            return data.Length >= key.Length && data.Span.Slice(0, key.Length).SequenceEqual(key.Span);
        }


        public byte[] PackPrevConnect(Memory<byte> key, out int length)
        {
            length =
               +key.Length //key
               + 1 // 0000 0000 address type + command
               + 1 + TargetAddress.Length + 2;// target length


            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory(0, length);
            int index = 0;

            key.CopyTo(memory.Slice(index));
            index += key.Length;

            //协议内容
            bytes[index] = (byte)((byte)AddressType << 4 | (byte)Command);
            index += 1;

            bytes[index] = (byte)TargetAddress.Length;
            index += 1;

            TargetAddress.CopyTo(memory.Slice(index));
            index += TargetAddress.Length;
            TargetPort.ToBytes(memory.Slice(index));
            index += 2;

            return bytes;
        }
        public byte[] PackConnect(Memory<byte> connectData, int connectDataLength, Memory<byte> data, int padding, out int length)
        {
            length = connectDataLength + 4 + data.Length + padding;

            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory();

            int index = 0;
            connectData.Slice(0, connectDataLength).CopyTo(memory.Slice(index));
            index += connectDataLength;

            data.Length.ToBytes(memory.Slice(index));
            index += 4;

            data.CopyTo(memory.Slice(index));
            index += data.Length;

            return bytes;
        }
        public bool UnPackConnect(Memory<byte> data, Memory<byte> key)
        {
            Span<byte> span = data.Span;

            int index = key.Length;

            AddressType = (Socks5EnumAddressType)(span[index] >> 4);
            Command = (Socks5EnumRequestCommand)(span[index] & 0b0000_1111);
            index += 1;


            byte targetepLength = span[index];
            index += 1;
            TargetAddress = data.Slice(index, targetepLength);
            index += targetepLength;
            TargetPort = span.Slice(index, 2).ToUInt16();
            index += 2;

            int length = span.Slice(index, 4).ToInt32();
            index += 4;

            Data = data.Slice(index, length);

            return true;
        }

        public static byte[] PackFirstResponse(Memory<byte> data, int padding, out int length)
        {
            length = 4 + data.Length + padding;

            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory();

            int index = 0;
            data.Length.ToBytes(memory.Slice(index));
            index += 4;

            data.CopyTo(memory.Slice(index));
            index += data.Length;

            return bytes;
        }
        public static Memory<byte> UnPackFirstResponse(Memory<byte> data)
        {
            int length = data.ToInt32();

            data = data.Slice(4, length);
            return data;
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
}
