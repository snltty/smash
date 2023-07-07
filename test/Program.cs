using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {

            BenchmarkRunner.Run<Test>();
        }
    }

    [MemoryDiagnoser]
    public partial class Test
    {
        Dictionary<string, bool> dic = new Dictionary<string, bool>();
        uint[][] arr = new uint[][] { 
            new uint[]{ 1,2},
            new uint[]{ 12,2},
            new uint[]{ 12,2},
            new uint[]{ 13,2},
            new uint[]{ 14,2},
            new uint[]{ 15,2},
            new uint[]{ 15,2},
            new uint[]{ 15,2},
            new uint[]{ 176,2},
            new uint[]{ 1,652},
            new uint[]{ 1,25},
            new uint[]{ 15,26},
            new uint[]{ 15,52},
            new uint[]{ 15,52},
            new uint[]{ 15,52},
            new uint[]{ 15,52},
            new uint[]{ 15,52},
            new uint[]{ 15,52},
            new uint[]{ 15,52},
            new uint[]{ 15,52},
        };
        [GlobalSetup]
        public void Startup()
        {
            for (uint i = 0; i < 100; i++)
            {
                dic.Add(i.ToString(),true);
            }
        }


        [Benchmark]
        public void Dic()
        {
           dic.ContainsKey("127.0.0.1");
        }

        int a = 1;
        int b = 100;
        [Benchmark]
        public void For()
        {

            for (int i = 0; i < 20; i++)
            {
                if ((arr[i][0]) == 0)
                {

                }
            }
        }
    }

}