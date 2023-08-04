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
            // BenchmarkRunner.Run<Test>();

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
            saea.SetBuffer(new byte[8 * 1024]);
        }

        SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
        [Benchmark]
        public void SetBuffer()
        {
            saea.SetBuffer(saea.Buffer, 1024, 8 * 1024);

        }

    }

}