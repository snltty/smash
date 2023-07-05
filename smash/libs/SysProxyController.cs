using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace smash.libs
{
    public sealed class SysProxyController
    {
        private readonly Config config;
        private bool isPac;
        private bool isEnv;

        public SysProxyController(Config config)
        {
            this.config = config;

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Stop();
            };
        }
        public void Start()
        {
            if (config.UseHijack == false && config.SysProxy == null) return;

            if (config.SysProxy.IsPac)
            {
                isPac = true;
                SetPac(config.SysProxy.Pac);
            }
            if (config.SysProxy.IsEnv)
            {
                isEnv = true;
                SetEnv($"socks5://{config.Proxy.Host}:{config.Proxy.Port}", config.Proxy.UserName, config.Proxy.Password);
            }
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
            Command.Windows(string.Empty, new string[] { $"setx http_proxy \"{proxyUrl}\" -m", $"setx https_proxy \"{proxyUrl}\" -m" });
            if (string.IsNullOrWhiteSpace(username) == false)
            {
                Command.Windows(string.Empty, new string[] {
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
            Command.Windows(string.Empty, new string[] {
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
        static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        const int INTERNET_OPTION_REFRESH = 37;
        private static void FlushOs()
        {
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
    }
}
