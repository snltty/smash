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
        static unsafe void Main(string[] args)
        {

            ProxyClientConfig proxyClientConfig = new ProxyClientConfig();
            proxyClientConfig.Key = "abcdefg";

            ProxyInfo proxyInfo = new ProxyInfo { AddressType = Socks5EnumAddressType.Domain, Command = Socks5EnumRequestCommand.Connect, TargetAddress = Encoding.UTF8.GetBytes("www.baidu.com"), TargetPort = 443 };

            byte[] bytes = proxyInfo.PackConnect(proxyClientConfig.KeyMemory, out int length);

            ProxyInfo proxyInfo1 = new ProxyInfo();
            proxyInfo1.UnPackConnect(bytes.AsMemory(0,length), proxyClientConfig.KeyMemory);

            //  Test();
            // BenchmarkRunner.Run<Test>();

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