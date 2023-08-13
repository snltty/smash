using common.libs.database;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace smash.plugins.hijack
{
    [Table("hijack-config")]
    public sealed class HijackConfig
    {
        private readonly IConfigDataProvider<HijackConfig> configDataProvider;
        public HijackConfig() { }
        public HijackConfig(IConfigDataProvider<HijackConfig> configDataProvider)
        {
            this.configDataProvider = configDataProvider;
            HijackConfig _config = configDataProvider.Load().Result ?? new HijackConfig();
            Processs = _config.Processs;
            IntranetIpv4s = _config.IntranetIpv4s;
            Save();

        }

        /// <summary>
        /// 进程配置列表
        /// </summary>
        public List<ProcessInfo> Processs { get; set; } = new List<ProcessInfo> {
            new ProcessInfo{ Name="浏览器", TCP = true,UDP = true,DNS = true, Use = true, FileNames = new List<string>{"chrome.exe" } }
        };
        /// <summary>
        /// 阻止内网ip列表
        /// </summary>
        public List<string> IntranetIpv4s = new List<string>() {
            "10.0.0.0/8", "100.64.0.0/10",
            "127.0.0.0/8", "169.254.0.0/16", "172.16.0.0/12",
            "192.0.0.0/24", "192.0.2.0/24","192.88.99.0/24","192.168.0.0/16",
            "198.18.0.0/15","198.51.100.0/24",
            "203.0.113.0/24","224.0.0.0/4", "240.0.0.0/4","255.255.255.255/32"
        };

        /// <summary>
        /// 当前进程列表文件名，方便查询
        /// </summary>
        [JsonIgnore]
        public ProcessParseInfo[] CurrentProcesss { get; private set; } = Array.Empty<ProcessParseInfo>();
        public void ParseProcesss()
        {
            List<ProcessParseInfo> res = new List<ProcessParseInfo>();
            foreach (var process in Processs.Where(c => c.Use))
            {
                foreach (var item in process.FileNames)
                {
                    res.Add(new ProcessParseInfo
                    {
                        Name = item,
                        Options = process
                    });
                }
            }
            CurrentProcesss = res.ToArray();
        }

        public void Save()
        {
            configDataProvider.Save(this).Wait();
        }
    }

    public sealed class ProcessParseInfo
    {
        public string Name { get; set; }
        public ProcessInfo Options { get; set; }
    }

    public sealed class ProcessInfo
    {
        /// <summary>
        /// 是否进程劫持
        /// </summary>
        public bool Use { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 处理TCP
        /// </summary>
        public bool TCP { get; set; } = true;
        /// <summary>
        /// 处理UDP
        /// </summary>
        public bool UDP { get; set; } = true;
        /// <summary>
        /// 处理所有DNS查询包
        /// </summary>
        public bool DNS { get; set; } = true;

        public List<string> FileNames { get; set; } = new List<string>();
    }
}
