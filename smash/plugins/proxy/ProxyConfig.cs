using common.libs.database;
using System.ComponentModel.DataAnnotations.Schema;

namespace smash.plugins.proxy
{
    [Table("proxy-config")]
    public sealed class ProxyConfig
    {
        private readonly IConfigDataProvider<ProxyConfig> configDataProvider;
        public ProxyConfig() { }
        public ProxyConfig(IConfigDataProvider<ProxyConfig> configDataProvider)
        {
            this.configDataProvider = configDataProvider;
            ProxyConfig _config = configDataProvider.Load().Result ?? new ProxyConfig();
            Proxy = _config.Proxy;
            Proxys = _config.Proxys;
            Save();
        }


        #region 代理
        /// <summary>
        /// 当前使用代理
        /// </summary>
        public ProxyInfo Proxy { get; set; } = new ProxyInfo { Use = true, Host = "127.0.0.1", Port = 5413, Name = "默认", Password = string.Empty, UserName = string.Empty };
        /// <summary>
        /// 代理配置列表
        /// </summary>
        public List<ProxyInfo> Proxys { get; set; } = new List<ProxyInfo> {
            new ProxyInfo{ Use = true, Host = "127.0.0.1", Port = 5413, Name = "默认", Password = string.Empty, UserName = string.Empty },
        };
        #endregion

        public void Save()
        {
            configDataProvider.Save(this).Wait();
        }
    }


    public sealed class ProxyInfo
    {
        public bool Use { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public long Delay { get; set; }
    }
}
