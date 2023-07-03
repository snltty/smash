using System.Diagnostics;
using System.Net;
using System.Text;
using static netch_process.libs.Redirector;

namespace netch_process.libs;

public class NFController
{
    private static readonly string SystemDriver = $"{Environment.SystemDirectory}\\drivers\\netfilter2.sys";
    public const string NFDriver = "nfdriver.sys";
    private readonly Config config;
    public NFController(Config config)
    {
        this.config = config;
    }

    public async Task StartAsync()
    {

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

public sealed class Config
{
    #region 劫持选项
    /// <summary>
    /// 处理TCP
    /// </summary>
    public bool FilterTCP { get; set; } = true;
    /// <summary>
    /// 处理UDP
    /// </summary>
    public bool FilterUDP { get; set; } = true;
    /// <summary>
    /// 处理ICMP
    /// </summary>
    public bool FilterICMP { get; set; } = false;
    /// <summary>
    /// ICMP延迟
    /// </summary>
    public int ICMPDelay { get; set; } = 10;


    /// <summary>
    /// 处理子进程
    /// </summary>
    public bool FilterParent { get; set; } = true;

    /// <summary>
    /// 处理环回
    /// </summary>
    public bool FilterLoopback { get; set; } = false;
    /// <summary>
    /// 处理内网
    /// </summary>
    public bool FilterIntranet { get; set; } = false;

    /// <summary>
    /// 处理被处理进程的DNS查询包
    /// </summary>
    public bool HandleOnlyDNS { get; set; } = false;
    /// <summary>
    /// 处理所有DNS查询包
    /// </summary>
    public bool FilterDNS { get; set; } = false;
    /// <summary>
    /// 是否代理处理DNS
    /// </summary>
    public bool DNSProxy { get; set; } = false;
    public string DNSHost { get; set; } = $"8.8.8.8";
    public int DNSPort { get; set; } = 53;
    #endregion

    #region 代理
    public ProxyInfo Proxy { get; set; } = new ProxyInfo { Host = "127.0.0.1", Port = 5413, Name = "默认", Password = string.Empty, UserName = string.Empty };
    public List<ProxyInfo> Proxys { get; set; } = new List<ProxyInfo> {
        new ProxyInfo{ Host = "127.0.0.1", Port = 5413, Name = "默认", Password = string.Empty, UserName = string.Empty },
    };
    #endregion

    #region 进程
    public ProcessInfo Process { get; set; } = new ProcessInfo { Name = "浏览器", FileNames = new List<string> { "chrome.exe" } };
    public List<ProcessInfo> Processs { get; set; } = new List<ProcessInfo> {
         new ProcessInfo{ Name="浏览器", FileNames = new List<string>{"chrome.exe" } }
    };
    #endregion


    public bool Running { get; set; }


    static string fileName = "config.json";
    public void Save()
    {
        File.WriteAllText(fileName, System.Text.Json.JsonSerializer.Serialize(this));
    }
    public static Config Load()
    {
        if (File.Exists(fileName) == false)
        {
            return new Config();
        }
        return System.Text.Json.JsonSerializer.Deserialize<Config>(File.ReadAllText(fileName));
    }
}


public sealed class ProxyInfo
{
    public string Name { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public long Delay { get; set; }
}

public sealed class ProcessInfo
{
    public string Name { get; set; }
    public List<string> FileNames { get; set; } = new List<string>();
}