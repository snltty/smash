using common.libs;
using System.Collections.Generic;

namespace smash.proxy.argument
{
    public static class ArgumentParser
    {
        public static Dictionary<string, string> Parse(string[] args, out string error)
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

            Validate(dic, out error);

            return dic;
        }
        static bool Validate(Dictionary<string, string> dic, out string error)
        {
            error = string.Empty;

            return ValidateMode(dic) &&
             ValidateServer(dic, out error) &&
             ValidateFake(dic, out error) &&
             ValidatePort(dic) &&
             ValidateKey(dic, out error) &&
             ValidateBuff(dic);
        }
        static bool ValidateMode(Dictionary<string, string> dic)
        {
            //模式
            if (dic.ContainsKey("mode") == false || dic["mode"] != "client" && dic["mode"] != "server")
            {
                dic["mode"] = "server";
            }
            return true;
        }
        static bool ValidateServer(Dictionary<string, string> dic, out string error)
        {
            error = string.Empty;
            //服务器地址
            if (dic["mode"] == "client")
            {
                if (dic.ContainsKey("server") == false || string.IsNullOrWhiteSpace(dic["server"]))
                {
#if DEBUG
                    dic["server"] = "127.0.0.1:5413";
#else
                    error = $"client mode need server endpoint";
                    return false;
#endif
                }
            }
            return true;
        }
        static bool ValidateFake(Dictionary<string, string> dic, out string error)
        {
            error = string.Empty;
            if (dic["mode"] == "server")
            {
                //伪装地址
                if (dic.ContainsKey("fake") == false || string.IsNullOrWhiteSpace(dic["fake"]))
                {
#if DEBUG
                    dic["fake"] = "127.0.0.1:8200";
#else
                    error = $"server mode need fake endpoint";
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
        static bool ValidateKey(Dictionary<string, string> dic, out string error)
        {
            error = string.Empty;
            //密钥
            if (dic.ContainsKey("key") == false || string.IsNullOrWhiteSpace(dic["key"]))
            {
#if DEBUG
                dic["key"] = "SNLTTY";
#else
                if (dic["mode"] == "client")
                {
                    error = $"client mode need server key";
                    return false;
                }
                else
                {
                    dic["key"] = Helper.RandomPasswordString(8);
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
