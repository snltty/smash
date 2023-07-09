using Microsoft.Extensions.DependencyInjection;
using smash.plugin;
using System.Reflection;

namespace smash.plugins.proxy
{
    internal class Plugin : IPlugin
    {
        public void LoadAfter(ServiceProvider services, Assembly[] assemblys)
        {
        }

        public void LoadBefore(ServiceCollection services, Assembly[] assemblys)
        {
            services.AddSingleton<ProxyConfig>();
            services.AddSingleton<ProxySettingForm>();
        }
    }
}
