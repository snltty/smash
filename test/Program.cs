using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
           // BenchmarkRunner.Run<Test>();
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