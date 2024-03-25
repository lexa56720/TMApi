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
        public static int LongPollPort { get; set; }


        public static TimeSpan LongPollLifeTime { get; set; }
        public static TimeSpan TokenLifeTime { get; set; }
        public static TimeSpan RsaLifeTime { get; set; }


        public static string TMDBConnectionString { get; set; } = string.Empty;
        public static string ImagesDBConnectionString { get; set; } = string.Empty;

        public static string PasswordSalt { get; set; } = string.Empty;
        public static int Version { get; set; }

        static GlobalSettings()
        {
            Reload();
        }

        public static void Reload()
        {
            AuthPort = ServerConfig.Default.AuthPort;
            ApiPort = ServerConfig.Default.ApiPort;
            LongPollPort = ServerConfig.Default.LongPollPort;

            LongPollLifeTime = TimeSpan.FromSeconds(ServerConfig.Default.LongPollLifeTimeSeconds);
            TokenLifeTime = TimeSpan.FromHours(ServerConfig.Default.TokenLifetimeHours);
            RsaLifeTime = TimeSpan.FromHours(ServerConfig.Default.RsaKeyLifetimeHours);

            TMDBConnectionString = ServerConfig.Default.TMDBConnectionString;
            ImagesDBConnectionString = ServerConfig.Default.TMImagesDBConnetctionString;
            PasswordSalt = ServerConfig.Default.PasswordSalt;
            Version = GetVersion(ServerConfig.Default.Version);
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
