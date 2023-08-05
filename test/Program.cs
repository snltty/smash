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
            BenchmarkRunner.Run<Test>();

        }



    }

    [MemoryDiagnoser]
    public partial class Test
    {
        [GlobalSetup]
        public void Startup()
        {
        }

        Random rd = new Random();
        [Benchmark]
        public void TestFunc()
        {
            rd.Next(100,1024);
        }


    }

}