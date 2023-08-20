using common.libs;
using common.libs.extends;
using Microsoft.Win32;
using smash.plugin;
using smash.plugins.proxy;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using File = System.IO.File;

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

            PacServer();
        }

        public bool Validate(out string error)
        {
            error = string.Empty;
            sysProxyConfig.SysProxy = sysProxyConfig.SysProxys.FirstOrDefault(c => c.Use);
            if (sysProxyConfig.SysProxy == null)
            {
                return false;
            }
            if (sysProxyConfig.SysProxy.IsPac == false && sysProxyConfig.SysProxy.IsEnv == false)
            {
                error = $"系统代理:未选择pac代理或环境变量中的任意一种";
                return false;
            }
            if (string.IsNullOrWhiteSpace(sysProxyConfig.SysProxy.Pac))
            {
                error = $"系统代理:未选择任何pac文件";
                return false;
            }

            return true;
        }
        public bool Start()
        {
            if (sysProxyConfig.SysProxy.IsPac)
            {
                isPac = true;
                SetPac(sysProxyConfig.SysProxy.Pac);
            }
            if (sysProxyConfig.SysProxy.IsEnv)
            {
                isEnv = true;
                SetEnv($"socks5h://{proxyConfig.Proxy.Host}:{proxyConfig.Proxy.Port}", proxyConfig.Proxy.UserName, proxyConfig.Proxy.Password);
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


        private void PacServer()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    HttpListener http;
                    while (true)
                    {
                        try
                        {
                            sysProxyConfig.PacServerPort = (ushort)new Random().Next(10000, 50000);
                            http = new HttpListener();
                            http.Prefixes.Add($"http://+:{sysProxyConfig.PacServerPort}/");
                            http.Start();

                            break;
                        }
                        catch (Exception)
                        {
                        }
                    }


                    while (true)
                    {
                        HttpListenerContext context = http.GetContext();
                        HttpListenerRequest request = context.Request;
                        using HttpListenerResponse response = context.Response;
                        using Stream stream = response.OutputStream;

                        try
                        {
                            response.Headers.Set("Server", "snltty");

                            string path = request.Url.AbsolutePath;
                            //默认页面
                            if (path == "/") path = "default.pac";

                            byte[] bytes = Read(path, out DateTime last);
                            if (bytes.Length > 0)
                            {
                                response.ContentLength64 = bytes.Length;
                                response.ContentType = GetContentType(path);
                                //response.Headers.Set("Last-Modified", last.ToString());
                                stream.Write(bytes, 0, bytes.Length);
                            }
                            else
                            {
                                response.StatusCode = (int)HttpStatusCode.NotFound;
                            }
                        }
                        catch (Exception)
                        {
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        stream.Close();
                        stream.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            }, TaskCreationOptions.LongRunning);
        }
        private byte[] Read(string fileName, out DateTime lastModified)
        {
            fileName = Path.Join(sysProxyConfig.PacRoot, fileName);
            lastModified = DateTime.UtcNow;
            if (File.Exists(fileName))
            {
                lastModified = File.GetLastWriteTimeUtc(fileName);
                return File.ReadAllBytes(fileName);
            }
            return Helper.EmptyArray;
        }
        private string GetContentType(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext == ".pac")
            {
                return "application/x-ns-proxy-autoconfig; charset=utf-8";
            }
            return "application/octet-stream";
        }


        private void SetPac(string pacUrl)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    IPAddress ipAddress = NetworkHelper.GetDomainIp(proxyConfig.Proxy.Host);
                    string pacContent = File.ReadAllText(Path.Join(sysProxyConfig.PacRoot, pacUrl)).Replace("{proxy}", $"SOCKS5 {ipAddress}:{proxyConfig.Proxy.Port}");
                    File.WriteAllText(Path.Join(sysProxyConfig.PacRoot, "--socks.pac"), pacContent);

                    string[] names = GetWindowsCurrentIds();
                    foreach (var item in names)
                    {
                        try
                        {
                            RegistryKey reg = Registry.Users.OpenSubKey($"{item}\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                            reg.SetValue("AutoConfigURL", $"http://{IPAddress.Loopback}:{sysProxyConfig.PacServerPort}/--socks.pac");
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
