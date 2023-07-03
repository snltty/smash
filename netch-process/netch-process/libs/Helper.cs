using System.Net;

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
    }
}
