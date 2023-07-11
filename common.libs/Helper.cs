using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace common.libs
{
    public static class Helper
    {
        public static byte[] EmptyArray = Array.Empty<byte>();
        public static byte[] TrueArray = new byte[] { 1 };
        public static byte[] FalseArray = new byte[] { 0 };
        public static byte[] AnyIpArray = IPAddress.Any.GetAddressBytes();
        public static byte[] AnyIpv6Array = IPAddress.IPv6Any.GetAddressBytes();
        public static byte[] AnyPoryArray = new byte[] { 0, 0 };

        public static async Task Await()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => cancellationTokenSource.Cancel();
            Console.CancelKeyPress += (sender, e) => cancellationTokenSource.Cancel();
            await Task.Delay(-1, cancellationTokenSource.Token);
        }
    }
}
