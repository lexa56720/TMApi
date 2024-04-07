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
        public static int InfoPort { get; set; }

        public static TimeSpan LongPollLifeTime { get; set; }
        public static TimeSpan TokenLifeTime { get; set; }
        public static TimeSpan RsaLifeTime { get; set; }

        public static string TMDBConnectionString { get; set; } = string.Empty;
        public static string FilesDBConnectionString { get; set; } = string.Empty;

        public static string PasswordSalt { get; set; } = string.Empty;
        public static int Version { get; set; }
        public static TimeSpan OnlineTimeout { get; set; }

        static GlobalSettings()
        {
            Reload();
        }

        public static void Reload()
        {
            InfoPort = ServerConfig.Default.InfoPort;

            LongPollLifeTime = TimeSpan.FromSeconds(ServerConfig.Default.LongPollLifeTimeSeconds);
            TokenLifeTime = TimeSpan.FromHours(ServerConfig.Default.TokenLifetimeHours);
            RsaLifeTime = TimeSpan.FromHours(ServerConfig.Default.RsaKeyLifetimeHours);

            TMDBConnectionString = ServerConfig.Default.TMDBConnectionString;
            FilesDBConnectionString = ServerConfig.Default.TMDBFilesConnetctionString;
            PasswordSalt = ServerConfig.Default.PasswordSalt;
            Version = ServerConfig.Default.Version;
            OnlineTimeout = 2 * LongPollLifeTime;
        }
    }
}
