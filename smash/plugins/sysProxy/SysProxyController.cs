using common.libs;
using Microsoft.Win32;
using smash.plugin;
using smash.plugins.proxy;
using System.Runtime.InteropServices;

namespace smash.plugins.sysProxy
{
    public sealed class SysProxyController : IController
    {
        private readonly SysProxyConfig sysProxyConfig;
        private readonly ProxyConfig proxyConfig;
        private bool isPac;
        private bool isEnv;

        public SysProxyController(SysProxyConfig sysProxyConfig, ProxyConfig proxyConfig)
        {
            this.sysProxyConfig = sysProxyConfig;
            this.proxyConfig = proxyConfig;

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Stop();
            };
        }
        public bool Validate(out string error)
        {
            error = string.Empty;
            if (sysProxyConfig.SysProxy == null) return false;

            if (sysProxyConfig.SysProxy.IsPac == false && sysProxyConfig.SysProxy.IsEnv == false)
            {
                error = $"系统代理:至少选择一种方式";
            }

            return true;
        }
        public bool Start()
        {
            if (sysProxyConfig.SysProxy == null) return false;
            if (sysProxyConfig.SysProxy.IsPac)
            {
                isPac = true;
                SetPac(sysProxyConfig.SysProxy.Pac);
            }
            if (sysProxyConfig.SysProxy.IsEnv)
            {
                isEnv = true;
                SetEnv($"socks5://{proxyConfig.Proxy.Host}:{proxyConfig.Proxy.Port}", proxyConfig.Proxy.UserName, proxyConfig.Proxy.Password);
            }
            return true;
        }
        public void Stop()
        {
            if (isPac)
            {
                isPac = false;
                ClearPac();
            }
            if (isEnv)
            {
                isEnv = false;
                ClearEnv();
            }
        }


        private void SetPac(string pacUrl)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string[] names = GetWindowsCurrentIds();
                    foreach (var item in names)
                    {
                        try
                        {
                            RegistryKey reg = Registry.Users.OpenSubKey($"{item}\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                            reg.SetValue("AutoConfigURL", pacUrl);
                            reg.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    FlushOs();
                }
            }
            catch (Exception)
            {
            }
        }
        private void SetEnv(string proxyUrl, string username, string password)
        {
            CommandHelper.Windows(string.Empty, new string[] { $"setx http_proxy \"{proxyUrl}\" -m", $"setx https_proxy \"{proxyUrl}\" -m" });
            if (string.IsNullOrWhiteSpace(username) == false)
            {
                CommandHelper.Windows(string.Empty, new string[] {
                    $"setx http_proxy_user {username} -m",
                    $"setx http_proxy_pass {password} -m",
                    $"setx https_proxy_user {username} -m",
                    $"setx https_proxy_pass {password} -m",
               });
            }
        }

        private void ClearPac()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string[] names = GetWindowsCurrentIds();
                    foreach (var item in names)
                    {
                        try
                        {
                            RegistryKey reg = Registry.Users.OpenSubKey($"{item}\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                            reg.DeleteValue("AutoConfigURL");
                            reg.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }


                    FlushOs();
                }
            }
            catch (Exception)
            {
            }
        }
        private void ClearEnv()
        {
            CommandHelper.Windows(string.Empty, new string[] {
                $"setx http_proxy \"\" -m",
                $"setx https_proxy \"\" -m",
            });
        }


        private static string[] GetWindowsCurrentIds()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Registry.Users.GetSubKeyNames().Where(c => c.Length > 10 && c.Contains("Classes") == false).ToArray();
            }
            return Array.Empty<string>();
        }
        [DllImport("wininet.dll")]
        static extern bool InternetSetOption(nint hInternet, int dwOption, nint lpBuffer, int dwBufferLength);
        const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        const int INTERNET_OPTION_REFRESH = 37;
        private static void FlushOs()
        {
            InternetSetOption(nint.Zero, INTERNET_OPTION_SETTINGS_CHANGED, nint.Zero, 0);
            InternetSetOption(nint.Zero, INTERNET_OPTION_REFRESH, nint.Zero, 0);
        }


    }
}
