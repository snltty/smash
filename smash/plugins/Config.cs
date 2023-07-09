namespace smash.plugins
{
    public sealed class Config
    {
        public bool Running { get; set; }
      
       

        /// <summary>
        /// 配置文件
        /// </summary>
        static string fileName = "config.json";
        /// <summary>
        /// 保存配置
        /// </summary>
        public void Save()
        {
            File.WriteAllText(fileName, System.Text.Json.JsonSerializer.Serialize(this));
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        public static Config Load()
        {
            if (File.Exists(fileName) == false)
            {
                return new Config();
            }
            return System.Text.Json.JsonSerializer.Deserialize<Config>(File.ReadAllText(fileName));
        }
    }
}
