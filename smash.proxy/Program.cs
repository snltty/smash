using common.libs;
using common.libs.extends;
using smash.proxy.server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace smash.proxy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (a, b) =>
            {
                Logger.Instance.Error(b.ExceptionObject + "");
            };
            ThreadPool.SetMinThreads(1024, 1024);
            ThreadPool.SetMaxThreads(65535, 65535);
            LoggerConsole();

            Dictionary<string, string> dic = ParseParams(args);
            if (ValidateParams(dic))
            {
                try
                {
                    Run(dic);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            }

            await Helper.Await();
        }

        static void Run(Dictionary<string, string> dic)
        {
            switch (dic["mode"])
            {
                case "client":
                    {
                        Logger.Instance.Info($"smash client are running");
                        ProxyClientConfig proxyClientConfig = new ProxyClientConfig
                        {
                            BufferSize = (EnumBufferSize)byte.Parse(dic["buff"]),
                            Key = dic["key"],
                            ListenPort = ushort.Parse(dic["port"]),
                            ServerEP = IPEndPoint.Parse(dic["server"])
                        };
                        ProxyClient proxyClient = new ProxyClient(proxyClientConfig);
                        proxyClient.Start();

                        Logger.Instance.Info(string.Empty.PadLeft(32, '='));
                        Logger.Instance.Info($"listen 0.0.0.0:{proxyClientConfig.ListenPort}");
                        Logger.Instance.Info($"server {proxyClientConfig.ServerEP}");
                        Logger.Instance.Info($"buff {proxyClientConfig.BufferSize}");
                        Logger.Instance.Info($"key {proxyClientConfig.KeyMemory.GetString()}");
                        Logger.Instance.Info(string.Empty.PadLeft(32, '='));
                    }
                    break;
                case "server":
                    {
                        Logger.Instance.Info($"smash server are running");
                        ProxyServerConfig proxyServerConfig = new ProxyServerConfig
                        {
                            BufferSize = (EnumBufferSize)byte.Parse(dic["buff"]),
                            Key = dic["key"],
                            ListenPort = ushort.Parse(dic["port"]),
                            FakeEP = IPEndPoint.Parse(dic["fake"])
                        };
                        ProxyServer proxyServer = new ProxyServer(proxyServerConfig);
                        proxyServer.Start();

                        Logger.Instance.Info(string.Empty.PadLeft(32, '='));
                        Logger.Instance.Info($"listen 0.0.0.0:{proxyServerConfig.ListenPort}");
                        Logger.Instance.Info($"fake {proxyServerConfig.FakeEP}");
                        Logger.Instance.Info($"buff {proxyServerConfig.BufferSize}");
                        Logger.Instance.Info($"key {proxyServerConfig.KeyMemory.GetString()}");
                        Logger.Instance.Info(string.Empty.PadLeft(32, '='));
                    }
                    break;
                default:
                    Logger.Instance.Warning($"smash nothing is running");
                    break;
            }
        }

        private static void LoggerConsole()
        {
            if (Directory.Exists("log") == false)
            {
                Directory.CreateDirectory("log");
            }
            Logger.Instance.OnLogger += (model) =>
            {
                ConsoleColor currentForeColor = Console.ForegroundColor;
                switch (model.Type)
                {
                    case LoggerTypes.DEBUG:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case LoggerTypes.INFO:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LoggerTypes.WARNING:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LoggerTypes.ERROR:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        break;
                }
                string line = $"[{model.Type,-7}][{model.Time:yyyy-MM-dd HH:mm:ss}]:{model.Content}";
                Console.WriteLine(line);
                Console.ForegroundColor = currentForeColor;

                try
                {
                    using StreamWriter sw = File.AppendText(Path.Combine("log", $"{DateTime.Now:yyyy-MM-dd}.log"));
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                catch (Exception)
                {
                }
            };
        }

        static Dictionary<string, string> ParseParams(string[] args)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].IndexOf("--") == 0)
                {
                    if (i + 1 < args.Length && args[i + 1].IndexOf("--") == -1)
                    {
                        dic.Add(args[i].Substring(2), args[i + 1]);
                        i++;
                    }
                    else
                    {
                        dic.Add(args[i].Substring(2), string.Empty);
                    }
                }
            }

            return dic;
        }
        static bool ValidateParams(Dictionary<string, string> dic)
        {
            return ValidateMode(dic) &&
             ValidateServer(dic) &&
             ValidateFake(dic) &&
             ValidatePort(dic) &&
             ValidateKey(dic) &&
             ValidateBuff(dic);
        }
        static bool ValidateMode(Dictionary<string, string> dic)
        {
            //模式
            if (dic.ContainsKey("mode") == false || (dic["mode"] != "client" && dic["mode"] != "server"))
            {
                dic["mode"] = "server";
            }
            return true;
        }
        static bool ValidateServer(Dictionary<string, string> dic)
        {
            //服务器地址
            if (dic["mode"] == "client")
            {
                if (dic.ContainsKey("server") == false || string.IsNullOrWhiteSpace(dic["server"]))
                {
#if DEBUG
                    dic["server"] = "127.0.0.1:5413";
#else
                    Logger.Instance.Error($"client mode need server endpoint");
                    return false;
#endif
                }
            }
            return true;
        }
        static bool ValidateFake(Dictionary<string, string> dic)
        {
            if (dic["mode"] == "server")
            {
                //伪装地址
                if (dic.ContainsKey("fake") == false || string.IsNullOrWhiteSpace(dic["fake"]))
                {
#if DEBUG
                    dic["fake"] = "127.0.0.1:8200";
#else
                    Logger.Instance.Error($"server mode need fake endpoint");
                    return false;
#endif

                }
            }

            return true;
        }
        static bool ValidatePort(Dictionary<string, string> dic)
        {
            //监听端口
            if (dic.ContainsKey("port") == false || ushort.TryParse(dic["port"], out ushort port) == false)
            {
#if DEBUG
                dic["port"] = dic["mode"] == "client" ? "5414" : "5413";
#else
                dic["port"] = "5413";
#endif
            }
            return true;
        }
        static bool ValidateKey(Dictionary<string, string> dic)
        {
            //密钥
            if (dic.ContainsKey("key") == false || string.IsNullOrWhiteSpace(dic["key"]))
            {
#if DEBUG
                dic["key"] = "SNLTTY";
#else
                if (dic["mode"] == "client")
                {
                    Logger.Instance.Error($"client mode need server key");
                    return false;
                }
                else
                {
                    dic["key"] = Guid.NewGuid().ToString().ToUpper().Substring(0, 8);
                }
#endif
            }
            return true;
        }
        static bool ValidateBuff(Dictionary<string, string> dic)
        {
            //连接缓冲区大小(buffsize)
            if (dic.ContainsKey("buff") == false || string.IsNullOrWhiteSpace(dic["buff"]) || byte.TryParse(dic["buff"], out byte buff) == false)
            {
                dic["buff"] = "3";
            }
            return true;
        }

    }
}