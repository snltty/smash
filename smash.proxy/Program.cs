using common.libs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace smash.proxy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Dictionary<string, string> dic = ParseParams(args);

            await Helper.Await();
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
            ValidateParams(dic);
            return dic;
        }
        static void ValidateParams(Dictionary<string, string> dic)
        {
            if (dic.ContainsKey("mode") == false || (dic["mode"] != "client" && dic["mode"] != "server"))
            {
                dic["mode"] = "server";
            }

            if (dic.ContainsKey("port") == false || ushort.TryParse(dic["port"], out ushort port) == false)
            {
                dic["port"] = "5415";
            }

            if (dic.ContainsKey("key") == false || string.IsNullOrWhiteSpace(dic["key"]))
            {
                dic["key"] = Guid.NewGuid().ToString().ToUpper().Substring(0, 8);
            }
        }
    }
}