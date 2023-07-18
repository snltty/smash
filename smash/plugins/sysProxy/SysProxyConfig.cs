using common.libs.database;
using System.ComponentModel.DataAnnotations.Schema;

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
            SysProxy = _config.SysProxy;
            SysProxys = _config.SysProxys;
            Save();
        }

        /// <summary>
        /// 是否设置系统代理
        /// </summary>
        public bool UseSysProxy { get; set; }

        #region 系统代理
        public SysProxyInfo SysProxy { get; set; } = new SysProxyInfo { Name = "默认", IsEnv = true, IsPac = true, Pac = "default.pac" };
        public List<SysProxyInfo> SysProxys { get; set; } = new List<SysProxyInfo>
        {
            new SysProxyInfo { Name="默认", IsEnv = true, IsPac = true, Pac="default.pac" }
        };
        #endregion

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
