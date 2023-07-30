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
            if (index < 0) return -1;

            int endIndex = span.Slice(index + contentLengthBytes.Length).IndexOf(lineEnd);
            if (endIndex < 0) return -1;

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
            while (true)
            {
                if (lastLength == 0)
                {
                    lastLength = GetContentLength(data);
                    //没找到content-length，继续接收
                    if (lastLength == -1)
                    {
                        lastLength = 0;
                        offset = data.Length;
                        return Helper.EmptyArray;
                    }
                    //content-length确实为0，重新接收
                    else if (lastLength == 0)
                    {
                        offset = 0;
                        return Helper.EmptyArray;
                    }
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
                    //截取数据部分
                    data = data.Slice(index + 4);
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
                        //剩下的数据。是下个包的，需要前移
                        if (data.Length > lastLength)
                            data.Slice(lastLength).CopyTo(receiveBuffer);

                        data = data.Slice(0, lastLength);
                        lastLength = 0;
                        offset = lastLength;
                        headerEnd = false;
                    }
                    else
                    {
                        lastLength -= data.Length;
                        offset = lastLength;
                    }
                    return data;
                }
            }
        }

    }
}
