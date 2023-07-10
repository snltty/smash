using common.libs;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace smash.plugin
{
    public interface IPlugin
    {
        void LoadBefore(ServiceCollection services, Assembly[] assemblys);
        void LoadAfter(ServiceProvider services, Assembly[] assemblys);
    }

    public interface ITabForm
    {
        public int Order { get; }
    }

    public static class PluginLoader
    {
        public static IPlugin[] Plugins { get; private set; }
        public static ITabForm[] TabForms { get; set; }
        public static IPlugin[] LoadBefore(ServiceCollection services, Assembly[] assemblys)
        {
            IEnumerable<Type> types = ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(IPlugin)).Distinct();
            Plugins = types.Select(c => (IPlugin)Activator.CreateInstance(c)).ToArray();

            foreach (var item in Plugins)
            {
                item.LoadBefore(services, assemblys);
            }
            return Plugins;
        }

        public static void LoadAfter(IPlugin[] plugins, ServiceProvider services, Assembly[] assemblys)
        {
            foreach (var item in plugins)
            {
                item.LoadAfter(services, assemblys);
            }
        }
    }
}
