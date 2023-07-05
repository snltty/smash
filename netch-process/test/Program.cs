using netch_process.libs;

namespace test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HijackController hijackController = new HijackController(new Config
            {
                DNSHost = "1.1.1.1",
                DNSPort = 53,
                DNSProxy = true,
                FilterDNS = true,
                FilterICMP = false,
                FilterUDP = true,
                FilterTCP = true,
                FilterIntranet = false,
                FilterLoopback = false,
                FilterParent = true,
                HandleOnlyDNS = true,
                ICMPDelay = 10,
                Process = new ProcessInfo { FileNames = new List<string> { "steamwebhelper.exe" } },
                Proxy = new ProxyInfo { Host = "127.0.0.1", Port = 5413, UserName = string.Empty, Password = string.Empty },
                UseHijack = true
            });

            await hijackController.StartAsync();

            Console.ReadLine();
        }
    }
}