using System.Buffers.Binary;
using System.Data;
using System.Net;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* 100.64.0.0/10
            memset(&rule, 0, sizeof(NF_RULE));
            rule.ip_family = AF_INET;
            inet_pton(AF_INET, "100.64.0.0", rule.remoteIpAddress);
            inet_pton(AF_INET, "255.192.0.0", rule.remoteIpAddressMask);
            rule.filteringFlag = NF_ALLOW;
            nf_addRule(&rule, FALSE);
             */
            uint ip = BitConverter.ToUInt32(IPAddress.Parse("100.64.0.0").GetAddressBytes());
            uint network = BinaryPrimitives.ReverseEndianness(uint.MaxValue << (32 - 10));

            Console.WriteLine(string.Join(",",BitConverter.GetBytes(ip&network)));
            Console.WriteLine("Hello, World!");
        }
    }
}