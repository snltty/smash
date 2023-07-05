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
        /// 处理ICMP
        /// </summary>
        public bool FilterICMP { get; set; } = false;
        /// <summary>
        /// ICMP延迟
        /// </summary>
        public int ICMPDelay { get; set; } = 10;


        /// <summary>
        /// 处理子进程
        /// </summary>
        public bool FilterParent { get; set; } = true;

        /// <summary>
        /// 处理环回
        /// </summary>
        public bool FilterLoopback { get; set; } = false;
        /// <summary>
        /// 处理内网
        /// </summary>
        public bool FilterIntranet { get; set; } = false;

        /// <summary>
        /// 处理被处理进程的DNS查询包
        /// </summary>
        public bool HandleOnlyDNS { get; set; } = false;
        /// <summary>
        /// 处理所有DNS查询包
        /// </summary>
        public bool FilterDNS { get; set; } = false;
        /// <summary>
        /// 是否代理处理DNS
        /// </summary>
        public bool DNSProxy { get; set; } = false;
        public string DNSHost { get; set; } = $"8.8.8.8";
        public int DNSPort { get; set; } = 53;
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
        public bool UseSysProxy{ get; set; }

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
}
