using System.Diagnostics;
using System.Net;
using System.Text;
using static netch_process.libs.Redirector;

namespace netch_process.libs;

public sealed class HijackController
{
    private static readonly string SystemDriver = $"{Environment.SystemDirectory}\\drivers\\netfilter2.sys";
    public const string NFDriver = "nfdriver.sys";
    private readonly Config config;
    public HijackController(Config config)
    {
        this.config = config;
    }

    public async Task StartAsync()
    {
        if (config.UseHijack == false) return;

        if (config.Proxy == null || string.IsNullOrWhiteSpace(config.Proxy.Host) || config.Proxy.Port == 0)
        {
            throw new Exception($"proxy invalid!");
        }
        if (config.Process == null || config.Process.FileNames.Count == 0)
        {
            throw new Exception($"process invalid!");
        }
        IPAddress ip = Helper.GetHostIp(config.Proxy.Host) ?? throw new Exception("proxy host invalid!");

        CheckDriver();

        Dial(NameList.AIO_FILTERLOOPBACK, config.FilterLoopback);
        Dial(NameList.AIO_FILTERINTRANET, config.FilterIntranet);
        Dial(NameList.AIO_FILTERPARENT, config.FilterParent && config.HandleOnlyDNS);
        Dial(NameList.AIO_FILTERICMP, config.FilterICMP);
        if (config.FilterICMP)
            Dial(NameList.AIO_ICMPING, config.ICMPDelay.ToString());

        Dial(NameList.AIO_FILTERTCP, config.FilterTCP);
        Dial(NameList.AIO_FILTERUDP, config.FilterUDP);

        // DNS
        Dial(NameList.AIO_FILTERDNS, config.FilterDNS);
        Dial(NameList.AIO_DNSONLY, config.HandleOnlyDNS);
        Dial(NameList.AIO_DNSPROX, config.DNSProxy);
        if (config.FilterDNS)
        {
            Dial(NameList.AIO_DNSHOST, config.DNSHost);
            Dial(NameList.AIO_DNSPORT, config.DNSPort.ToString());
        }

        // Server
        Dial(NameList.AIO_TGTHOST, ip.ToString());
        Dial(NameList.AIO_TGTPORT, config.Proxy.Port.ToString());
        Dial(NameList.AIO_TGTUSER, config.Proxy.UserName);
        Dial(NameList.AIO_TGTPASS, config.Proxy.Password);

        DialRule(config.Process.FileNames.ToArray(), Array.Empty<string>());

        if (!await InitAsync())
            throw new Exception("Redirector start failed.");
    }
    public Task StopAsync()
    {
        return FreeAsync();
    }

    public void ClearRule()
    {
        Dial(NameList.AIO_CLRNAME, "");
    }
    public void DialRule(string[] addNames, string[] passNames)
    {
        ClearRule();
        if (addNames != null)
        {
            foreach (var item in addNames)
            {
                Dial(NameList.AIO_ADDNAME, item);

            }
        }
        if (passNames != null)
        {
            foreach (var item in passNames)
            {
                Dial(NameList.AIO_BYPNAME, item);
            }
        }
        // Bypass Self
        Dial(NameList.AIO_BYPNAME, "^" + ToRegexString(AppDomain.CurrentDomain.BaseDirectory));
    }

    private string ToRegexString(string value)
    {
        var sb = new StringBuilder();
        foreach (var t in value)
        {
            var escapeCharacters = new[] { '\\', '*', '+', '?', '|', '{', '}', '[', ']', '(', ')', '^', '$', '.' };
            if (escapeCharacters.Any(s => s == t))
                sb.Append('\\');

            sb.Append(t);
        }

        return sb.ToString();
    }

    private static string GetFileVersion(string file)
    {
        if (File.Exists(file))
            return FileVersionInfo.GetVersionInfo(file).FileVersion ?? "";

        return "";
    }
    private static void CheckDriver()
    {
        var binFileVersion = GetFileVersion(NFDriver);
        var systemFileVersion = GetFileVersion(SystemDriver);

        if (!File.Exists(SystemDriver))
        {
            // Install
            InstallDriver();
            return;
        }

        var reinstall = false;
        if (Version.TryParse(binFileVersion, out var binResult) && Version.TryParse(systemFileVersion, out var systemResult))
        {
            if (binResult.CompareTo(systemResult) > 0)
                // Update
                reinstall = true;
            else if (systemResult.Major != binResult.Major)
                // Downgrade when Major version different (may have breaking changes)
                reinstall = true;
        }
        else
        {
            // Parse File versionName to Version failed
            if (!systemFileVersion.Equals(binFileVersion))
                // versionNames are different, Reinstall
                reinstall = true;
        }

        if (!reinstall)
            return;

        UninstallDriver();
        InstallDriver();
    }
    private static void InstallDriver()
    {
        if (!File.Exists(NFDriver))
            throw new Exception("builtin driver files missing, can't install NF driver");

        try
        {
            File.Copy(NFDriver, SystemDriver);
        }
        catch (Exception e)
        {
            throw new Exception($"Copy netfilter2.sys failed\n{e.Message}");
        }

        // 注册驱动文件
        if (aio_register("netfilter2"))
        {
            Console.WriteLine("Install netfilter2 driver finished");
        }
        else
        {
            Console.WriteLine("Register netfilter2 failed");
        }
    }
    public static bool UninstallDriver()
    {
        if (!File.Exists(SystemDriver))
            return true;

        aio_unregister("netfilter2");
        File.Delete(SystemDriver);

        return true;
    }

}

