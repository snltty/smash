using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using common.libs;
using common.libs.extends;
using Microsoft.Diagnostics.Tracing.Etlx;
using smash.proxy;
using smash.proxy.client;
using smash.proxy.server;
using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
          //  Test();
            BenchmarkRunner.Run<Test>();

        }


        static byte[] bytes = Encoding.UTF8.GetBytes($"GET / HTTP/1.1\r\naaa: bbb\r\nccc: ddd\r\ndddddddddddddddddddddddddddddddddddddddddd: dffffffffffffffffffffffffffffffffffffffffffffffffffff\r\nContent-Length: 123\r\n\r\n123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890---");
        private static void Test()
        {
            //>0表示已经找到了 content-length，拿到了长度
            int contentLength = 0;
            //是否已经找到了\r\n\r\n结束标记
            bool Headed = false;

            Memory<byte> buffer = new byte[8 * 1024];
            int length = ReadData(buffer);
            Memory<byte> data = buffer.Slice(0, length);
            do
            {
                //Console.WriteLine($"data:{Encoding.UTF8.GetString(data.Span)}");
                if (contentLength > 0)
                {
                    if (Headed)
                    {
                        Memory<byte> sendDta = data;
                        if (sendDta.Length > contentLength)
                        {
                            sendDta = sendDta.Slice(contentLength);
                            contentLength = 0;
                           
                        }
                        else
                        {
                            contentLength -= sendDta.Length;
                        }

                        Console.WriteLine(Encoding.UTF8.GetString(sendDta.Span));

                        data = data.Slice(sendDta.Length);

                        if(contentLength > 0)
                        {
                            length = ReadData(buffer.Slice(data.Length));
                            data = buffer.Slice(0, data.Length + length);
                        }
                        continue;

                    }
                    int headIndex = HttpParser.GetHeaderEndIndex(data);
                    Console.WriteLine($"headIndex:{headIndex}");
                    if (headIndex >= 0)
                    {
                        Headed = true;
                        data = data.Slice(headIndex + 4);
                        continue;
                    }
                    length = ReadData(buffer.Slice(data.Length));
                    data = buffer.Slice(0, data.Length + length);
                }
                else
                {
                    contentLength = HttpParser.GetContentLength(data);
                   // Console.WriteLine($"contentLength:{contentLength}");
                    if (contentLength > 0)
                    {
                        continue;
                    }
                    length = ReadData(buffer.Slice(data.Length));
                    data = buffer.Slice(0, data.Length + length);
                }

            } while (data.Length > 0);
            Console.WriteLine("//");
        }

        static int readIndex = 0;
        private static int ReadData(Memory<byte> data)
        {
            if (readIndex >= bytes.Length) return 0;

            int length = new Random().Next(5, 20);
            if (readIndex + length >= bytes.Length)
            {
                length = bytes.Length - readIndex;
            }
            bytes.AsMemory(readIndex, length).CopyTo(data);
            readIndex += length;

            return length;
        }


        private async Task ProxyTest()
        {

            ProxyClientConfig proxyClientConfig = new ProxyClientConfig();
            proxyClientConfig.Key = "SNLTTYSSS";

            ProxyInfo info = new ProxyInfo
            {
                AddressType = Socks5EnumAddressType.IPV4,
                Command = Socks5EnumRequestCommand.Connect,
                TargetAddress = new byte[] { 127, 0, 0, 2 },
                TargetPort = 880
            };
            byte[] proxy = info.PackConnect(proxyClientConfig.HttpRequestHeader, out int length);
            Console.WriteLine("==============================================");


            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("8.210.20.111"), 443);
            var socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ep);

            SslStream sslStream = new SslStream(new NetworkStream(socket), true, (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                return true;
            }, null);
            await sslStream.AuthenticateAsClientAsync(string.Empty);
            await sslStream.WriteAsync(proxy.AsMemory(0, length));

            byte[] bytes = new byte[1024];
            length = await sslStream.ReadAsync(bytes, 0, bytes.Length);

            Console.WriteLine(Encoding.UTF8.GetString(bytes.AsSpan(0, length)));
        }

    }

    [MemoryDiagnoser]
    public partial class Test
    {
        [GlobalSetup]
        public void Startup()
        {
            saea.SetBuffer(new byte[8*1024]);
        }

        SocketAsyncEventArgs saea = new SocketAsyncEventArgs(); 
         [Benchmark]
        public void SetBuffer()
        {
            saea.SetBuffer(saea.Buffer, 1024,8*1024);

        }

    }

}