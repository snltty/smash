namespace smash.libs
{
    public sealed class Config
    {
        #region 劫持选项
        /// <summary>
        /// 处理TCP
        /// </summary>
        public bool FilterTCP { get; set; } = true;
        /// <summary>
        /// 处理UDP
        /// </summary>
        public bool FilterUDP { get; set; } = true;

        /// <summary>
        /// 处理所有DNS查询包
        /// </summary>
        public bool FilterDNS { get; set; } = false;
        #endregion

        #region 代理
        public ProxyInfo Proxy { get; set; } = new ProxyInfo { Host = "127.0.0.1", Port = 5413, Name = "默认", Password = string.Empty, UserName = string.Empty };
        public List<ProxyInfo> Proxys { get; set; } = new List<ProxyInfo> {
            new ProxyInfo{ Host = "127.0.0.1", Port = 5413, Name = "默认", Password = string.Empty, UserName = string.Empty },
        };
        #endregion

        #region 进程
        public ProcessInfo Process { get; set; } = new ProcessInfo { Name = "浏览器", FileNames = new List<string> { "chrome.exe" } };
        public List<ProcessInfo> Processs { get; set; } = new List<ProcessInfo> {
            new ProcessInfo{ Name="浏览器", FileNames = new List<string>{"chrome.exe" } }
        };
        public string[] CurrentProcesss { get; private set; }  = Array.Empty<string>();
        public void ParseProcesss()
        {
            if (Process != null)
            {
                CurrentProcesss = Process.FileNames.ToArray();
            }
        }
        public List<string> IntranetIpv4s = new List<string>() {
            "0.0.0.0/8", "10.0.0.0/8", "100.64.0.0/10","127.0.0.0/8", "169.254.0.0/16", "172.16.0.0/12",
            "192.0.0.0/24", "192.0.2.0/24","192.88.99.0/24","192.168.0.0/16",
            "198.18.0.0/15","198.51.100.0/24",
            "203.0.113.0/24","224.0.0.0/4", "240.0.0.0/4","255.255.255.255/32"
        };

        #endregion

        #region 系统代理
        public SysProxyInfo SysProxy { get; set; } = new SysProxyInfo { Name = "默认", IsEnv = true, IsPac = true, Pac = "http://127.0.0.1:5411/socks.pac" };
        public List<SysProxyInfo> SysProxys { get; set; } = new List<SysProxyInfo>
        {
            new SysProxyInfo { Name="默认", IsEnv = true, IsPac = true, Pac="http://127.0.0.1:5411/socks.pac" }
        };
        #endregion

        public bool Running { get; set; }
        public bool UseHijack { get; set; }
        public bool UseSysProxy { get; set; }

        static string fileName = "config.json";
        public void Save()
        {
            File.WriteAllText(fileName, System.Text.Json.JsonSerializer.Serialize(this));
        }
        public static Config Load()
        {
            if (File.Exists(fileName) == false)
            {
                return new Config();
            }
            return System.Text.Json.JsonSerializer.Deserialize<Config>(File.ReadAllText(fileName));
        }
    }

    public sealed class ProxyInfo
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public long Delay { get; set; }
    }

    public sealed class ProcessInfo
    {
        public string Name { get; set; }
        public List<string> FileNames { get; set; } = new List<string>();
    }

    public sealed class SysProxyInfo
    {
        public string Name { get; set; }
        public bool IsPac { get; set; }
        public string Pac { get; set; }
        public bool IsEnv { get; set; }
    }

    public readonly struct Ip4Info
    {
        public readonly uint Network;
        public readonly uint Mask;
        public Ip4Info(uint network, uint mask)
        {
            Network = network; Mask = mask;
        }
    }


}
