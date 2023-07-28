using System;
using System.Text;

namespace common.libs
{
    public static class HttpParser
    {
        private static byte[] contentLengthBytes = Encoding.ASCII.GetBytes("Content-Length: ");
        private static byte[] headerEnd = Encoding.UTF8.GetBytes("\r\n\r\n");
        private static byte[] lineEnd = Encoding.UTF8.GetBytes("\r\n");

        public static int GetContentLength(Memory<byte> memory)
        {
            Span<byte> span = memory.Span;
            int index = span.IndexOf(contentLengthBytes);
            if (index < 0) return 0;

            int endIndex = span.Slice(index + contentLengthBytes.Length).IndexOf(lineEnd);
            if (endIndex < 0) return 0;

            Span<byte> sps = span.Slice(index + contentLengthBytes.Length, endIndex);

            int value = 0;
            for (int i = 0; i < sps.Length; i++)
            {
                value = value * 10 + (sps[i] - 48);
            }

            return value;
        }
        public static int GetHeaderEndIndex(Memory<byte> memory)
        {
            return memory.Span.IndexOf(headerEnd);
        }
        public static int GetLineEndIndex(Memory<byte> memory)
        {
            return memory.Span.IndexOf(lineEnd);
        }
        public static Memory<byte> GetContentData(Memory<byte> data, byte[] receiveBuffer, ref int lastLength, ref bool headerEnd, out int offset)
        {

        restart:
            if (lastLength == 0)
            {
                lastLength = GetContentLength(data);
                //没找到content-length，去重新接收
                if (lastLength == 0)
                {
                    offset = data.Length;
                    return Helper.EmptyArray;
                }
                goto restart;
            }
            else if (headerEnd == false)
            {
                int index = GetHeaderEndIndex(data);
                //没找到\r\n\r\n去重新接收
                if (index < 0)
                {
                    offset = data.Length;
                    return Helper.EmptyArray;
                }
                headerEnd = true;
                //重新决定做什么
                data = data.Slice(index + 4);
                goto restart;
            }
            else
            {
                if (data.Length == 0)
                {
                    offset = 0;
                    return Helper.EmptyArray;
                }

                if (data.Length >= lastLength)
                {
                    //剩下的数据。是下个包的
                    data.Slice(lastLength).CopyTo(receiveBuffer);
                    offset = lastLength;

                    data = data.Slice(0, lastLength);
                    lastLength = 0;
                    headerEnd = false;
                }
                else
                {
                    lastLength -= data.Length;
                    offset = 0;
                }
            }

            return data;
        }

    }
}
