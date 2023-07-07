using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "D:\\Naraka\\program\\NeacClient.exe";
            string name = "\\NeacClient.exe";
            var span = path.AsSpan();
            var span1 = name.AsSpan();
            Console.WriteLine(path.AsSpan().Slice(span.Length - span1.Length, span1.Length).SequenceEqual(span1));

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

        string path = "D:\\Naraka\\program\\NeacClient.exe";
        string name = "\\NeacClient.exe";
        [Benchmark]
        public void Dic()
        {
            var span = path.AsSpan();
            var span1 = name.AsSpan();
            if(path.AsSpan().Slice(span.Length - span1.Length, span1.Length).SequenceEqual(span1))
            {

            }
        }

    }

}