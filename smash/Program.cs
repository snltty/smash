using common.libs.database;
using Microsoft.Extensions.DependencyInjection;
using smash.plugins;
using smash.plugin;
using System.Reflection;
using common.libs;
using System.Collections.Generic;

namespace smash
{
    internal static class Program
    {
        public static ServiceProvider serviceProvider = null;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Mutex mutex = new Mutex(true, System.Diagnostics.Process.GetCurrentProcess().ProcessName, out bool isAppRunning);
            if (isAppRunning == false)
            {
                Environment.Exit(1);
            }

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += Application_ThreadException;
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();


            ServiceCollection serviceCollection = new ServiceCollection();
            //注入 依赖注入服务供应 使得可以在别的地方通过注入的方式获得 ServiceProvider 以用来获取其它服务
            serviceCollection.AddSingleton((e) => serviceProvider);

            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();

            serviceCollection.AddSingleton<MainForm>();
            serviceCollection.AddSingleton<StartUpArgInfo>();
            serviceCollection.AddTransient(typeof(IConfigDataProvider<>), typeof(ConfigDataFileProvider<>));

            IEnumerable <Type> tabForms = ReflectionHelper.GetInterfaceSchieves(assemblys, typeof(ITabForm)).Distinct();
            foreach (Type item in tabForms)
            {
                serviceCollection.AddSingleton(item);
            }

            IPlugin[] plugins = PluginLoader.LoadBefore(serviceCollection, assemblys);
            serviceProvider = serviceCollection.BuildServiceProvider();
            PluginLoader.TabForms = tabForms.Select(c => (ITabForm)serviceProvider.GetService(c)).ToArray();

            PluginLoader.LoadAfter(plugins, serviceProvider, assemblys);


            StartUpArgInfo startUpArgInfo = serviceProvider.GetService<StartUpArgInfo>();
            startUpArgInfo.Args = args;
            Application.Run(serviceProvider.GetService<MainForm>());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"系统异常:{(e.ExceptionObject as Exception).Message}");
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"系统异常:{(e.Exception).Message}");
        }

    }

    public sealed class StartUpArgInfo
    {
        public string[] Args { get; set; }
    }
}