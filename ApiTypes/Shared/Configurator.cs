using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Shared
{
    public class Configurator
    {
        public Dictionary<string, string> ConfigData = new();
        public Configurator(string path, bool isEmbedded)
        {
            ConfigData = ConfigReader.Read(path, isEmbedded);
        }

        public T GetValue<T>(string key) where T : IParsable<T>
        {
            return T.Parse(ConfigData[key], null);
        }
        public string this[string key]
        {
            get { return ConfigData[key]; }
        }
    }
}
