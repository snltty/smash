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
            proxyConfig.Parse();
            if (proxyConfig.IPAddress == null || proxyConfig.Proxy.Port <= 0)
            {
                error = ($"代理:未选择代理，或代理信息不正确");
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
