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
        public static int AuthPort;

        public static int ApiPort;

        public static string ConnectionString;

        public static string PasswordSalt;

        public static int Version;
        static GlobalSettings()
        {
            Configurator configurator = new Configurator("config.cfg", false);
            AuthPort = configurator.GetValue<int>("auth-port");
            ApiPort = configurator.GetValue<int>("api-port");
            ConnectionString = configurator["connection-string"];
            PasswordSalt = configurator["password-salt"];
            Version = GetVersion( configurator["version"]);
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
