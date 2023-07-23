using common.libs.database;
using common.libs.extends;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace smash.plugins.sysProxy
{
    [Table("sysproxy-config")]
    public sealed class SysProxyConfig
    {
        private readonly IConfigDataProvider<SysProxyConfig> configDataProvider;
        public SysProxyConfig() { }
        public SysProxyConfig(IConfigDataProvider<SysProxyConfig> configDataProvider)
        {
            this.configDataProvider = configDataProvider;
            SysProxyConfig _config = configDataProvider.Load().Result ?? new SysProxyConfig();
            UseSysProxy = _config.UseSysProxy;
            SysProxy = _config.SysProxy;
            SysProxys = _config.SysProxys;
            Save();
        }

        /// <summary>
        /// 是否设置系统代理
        /// </summary>
        public bool UseSysProxy { get; set; }
        public SysProxyInfo SysProxy { get; set; } = new SysProxyInfo { Name = "默认", IsEnv = true, IsPac = true, Pac = "default.pac" };
        public List<SysProxyInfo> SysProxys { get; set; } = new List<SysProxyInfo>
        {
            new SysProxyInfo { Name="默认", IsEnv = true, IsPac = true, Pac="default.pac" }
        };

        [JsonIgnore]
        public string PacRoot { get; set; } = "./pacs/";
        [JsonIgnore]
        public ushort PacServerPort { get; set; }

        public void Save()
        {
            configDataProvider.Save(this).Wait();
        }
    }

    public sealed class SysProxyInfo
    {
        public string Name { get; set; }
        public bool IsPac { get; set; }
        public string Pac { get; set; }
        public bool IsEnv { get; set; }
    }
}
