using common.libs.database;
using System.ComponentModel.DataAnnotations.Schema;

namespace smash.plugins
{
    [Table("config")]
    public sealed class Config
    {
        public bool Running { get; set; }


        private readonly IConfigDataProvider<Config> configDataProvider;
        public Config() { }
        public Config(IConfigDataProvider<Config> configDataProvider)
        {
            this.configDataProvider = configDataProvider;
            Config _config = configDataProvider.Load().Result ?? new Config();
            Running = _config.Running;
        }

        public void Save()
        {
            configDataProvider.Save(this).Wait();
        }
    }
}
