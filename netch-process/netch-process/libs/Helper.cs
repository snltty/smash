using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace netch_process.libs
{
    internal static class Helper
    {
        public static IPAddress GetHostIp(string host)
        {
            if(IPAddress.TryParse(host,out IPAddress ip) == false)
            {
                try
                {
                    ip = Dns.GetHostEntry(host).AddressList[0];
                }
                catch (Exception)
                {
                }
            }
            return ip;
        }

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
        public static void FlushMemory()
        {
            GC.Collect();
            //GC.SuppressFinalize(true);
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
    }
}
