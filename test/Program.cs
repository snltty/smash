using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("8.210.20.111"),443);
            var socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ep);

            SslStream sslStream = new SslStream(new NetworkStream(socket),true, ValidateServerCertificate, null);
            await sslStream.AuthenticateAsClientAsync(string.Empty);
            await sslStream.WriteAsync(Encoding.UTF8.GetBytes($"GET / HTTP/1.1\r\nhost: proxy.snltty.com\r\n\r\n"));

            byte[] bytes = new byte[1024];
            int length = await sslStream.ReadAsync(bytes,0, bytes.Length);

            Console.WriteLine(Encoding.UTF8.GetString(bytes.AsSpan(0,length)));
           // BenchmarkRunner.Run<Test>();
        }

        public static bool ValidateServerCertificate( object sender,  X509Certificate certificate,  X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }

    [MemoryDiagnoser]
    public partial class Test
    {
        [GlobalSetup]
        public void Startup()
        {
        }
        [Benchmark]
        public void Dic()
        {
        }

    }

}