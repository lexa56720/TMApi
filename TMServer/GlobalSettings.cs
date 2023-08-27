using ApiTypes.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer
{
    internal static class GlobalSettings
    {
        public static int AuthPort;

        public static int ApiPort;

        public static string ConnectionString;

        public static string PasswordSalt;
        static GlobalSettings()
        {
            Configurator configurator = new Configurator("config.cfg", false);
            AuthPort = configurator.GetValue<int>("auth-port");
            ApiPort = configurator.GetValue<int>("api-port");
            ConnectionString = configurator["connection-string"];
            PasswordSalt = configurator["password-salt"];
        }
    }
}
