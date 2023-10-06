using ApiTypes.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMServer
{
    internal static class GlobalSettings
    {
        public static int AuthPort { get; set; }

        public static int ApiPort { get; set; }

        public static TimeSpan TokenLifeTime { get; set; }

        public static TimeSpan RsaLifeTime { get; set; }


        public static string ConnectionString { get; set; }

        public static string PasswordSalt { get; set; }

        public static int Version { get; set; }
        static GlobalSettings()
        {
            Reload();
        }

        public static void Reload()
        {
            Configurator configurator = new Configurator("config.cfg", false);
            AuthPort = configurator.GetValue<int>("auth-port");
            ApiPort = configurator.GetValue<int>("api-port");
            ConnectionString = configurator["connection-string"];
            TokenLifeTime = TimeSpan.FromHours(configurator.GetValue<int>("token-lifetime-hours"));
            RsaLifeTime = TimeSpan.FromHours(configurator.GetValue<int>("rsa-key-lifetime-hours"));
            PasswordSalt = configurator["password-salt"];
            Version = GetVersion(configurator["version"]);
        }
        private static int GetVersion(string ver)
        {
            var versionComponents = Regex.Replace(ver, "[^0-9.]", "").Split('.', StringSplitOptions.TrimEntries);
            var version = 0;
            for (int i = 0; i < versionComponents.Length; i++)
                version += int.Parse(versionComponents[i]) * (int)Math.Pow(10, versionComponents.Length - i - 1);
            return version;
        }
    }
}
