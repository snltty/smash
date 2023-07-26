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

    }
}
