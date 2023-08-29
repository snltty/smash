using common.libs;
using common.libs.extends;
using smash.proxy.argument;
using smash.proxy.client;
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

            Dictionary<string, string> dic = ArgumentParser.Parse(args, out string error);
            if (string.IsNullOrWhiteSpace(error) == false)
            {
                Logger.Instance.Error(error);
                return;
            }
            else
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
                        string[] arr = dic["server"].Split(':');
                        string port = arr.Length > 1 ? arr[1] : "443";
                        ProxyClientConfig proxyClientConfig = new ProxyClientConfig
                        {
                            BufferSize = (EnumBufferSize)byte.Parse(dic["buff"]),
                            Key = dic["key"],
                            ListenPort = ushort.Parse(dic["port"]),
                            Domain = arr[0],
                            ServerEP = IPEndPoint.Parse($"{NetworkHelper.GetDomainIp(arr[0])}:{port}")
                        };
                        ProxyClient proxyClient = new ProxyClient(proxyClientConfig);
                        proxyClient.Start();

                        Logger.Instance.Info(string.Empty.PadLeft(32, '='));
                        Logger.Instance.Info($"listen 0.0.0.0:{proxyClientConfig.ListenPort}");
                        Logger.Instance.Info($"server {dic["server"]}");
                        Logger.Instance.Info($"buff {proxyClientConfig.BufferSize}");
                        Logger.Instance.Info($"key {proxyClientConfig.KeyMemory.GetString()}");
                        Logger.Instance.Info($"time 2023-08-29 23:43");
                        Logger.Instance.Info(string.Empty.PadLeft(32, '='));
                    }
                    break;
                case "server":
                    {
                        Logger.Instance.Info($"smash server are running");
                        string[] arr = dic["fake"].Split(':');
                        string port = arr.Length > 1 ? arr[1] : "443";
                        ProxyServerConfig proxyServerConfig = new ProxyServerConfig
                        {
                            BufferSize = (EnumBufferSize)byte.Parse(dic["buff"]),
                            Key = dic["key"],
                            ListenPort = ushort.Parse(dic["port"]),
                            FakeEP = IPEndPoint.Parse($"{NetworkHelper.GetDomainIp(arr[0])}:{port}"),
                            Dns = IPAddress.Parse(dic["dns"])
                        };
                        ProxyServer proxyServer = new ProxyServer(proxyServerConfig);
                        proxyServer.Start();

                        Logger.Instance.Info(string.Empty.PadLeft(32, '='));
                        Logger.Instance.Info($"listen 0.0.0.0:{proxyServerConfig.ListenPort}");
                        Logger.Instance.Info($"fake {proxyServerConfig.FakeEP}");
                        Logger.Instance.Info($"buff {proxyServerConfig.BufferSize}");
                        Logger.Instance.Info($"key {proxyServerConfig.KeyMemory.GetString()}");
                        Logger.Instance.Info($"dns {proxyServerConfig.Dns}");
                        Logger.Instance.Info($"time 2023-08-29 23:43");
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
            //Logger.Instance.LoggerLevel = LoggerTypes.DEBUG;
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

    }
}