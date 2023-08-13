using common.libs;
using smash.plugin;

namespace smash.plugins.proxy
{
    public sealed class ProxyController : IController
    {
        private readonly ProxyConfig proxyConfig;
        public ProxyController(ProxyConfig proxyConfig)
        {
            this.proxyConfig = proxyConfig;
        }
        public bool Validate(out string error)
        {
            error = string.Empty;
            proxyConfig.Proxy = proxyConfig.Proxys.FirstOrDefault(c => c.Use);
            if (proxyConfig.Proxy == null || proxyConfig.Proxy.Port <= 0 || NetworkHelper.GetDomainIp(proxyConfig.Proxy.Host) == null)
            {
                error = ($"代理信息:未选择代理，或代理信息不正确");
                return false;
            }
            return true;
        }
        public bool Start()
        {
            return true;
        }
        public void Stop()
        {
        }
    }
}
