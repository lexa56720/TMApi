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
        public static int Version => ServerConfig.Default.Version;

        public static int InfoPort => ServerConfig.Default.InfoPort;

        public static TimeSpan LongPollLifeTime => TimeSpan.FromSeconds(ServerConfig.Default.LongPollLifeTimeSeconds);
        public static TimeSpan TokenLifeTime => TimeSpan.FromHours(ServerConfig.Default.TokenLifetimeHours);
        public static TimeSpan RsaLifeTime => TimeSpan.FromHours(ServerConfig.Default.RsaKeyLifetimeHours);

        public static string TMDBConnectionString => ServerConfig.Default.TMDBConnectionString;
        public static string FilesDBConnectionString => ServerConfig.Default.TMDBFilesConnetctionString;

        public static string PasswordSalt => ServerConfig.Default.PasswordSalt;
        public static TimeSpan OnlineTimeout => 2 * LongPollLifeTime;

        public static int MaxFileSizeMB=>ServerConfig.Default.FileMaxSizeMB;
        public static int MaxAttachments => ServerConfig.Default.MessageMaxFIles;

        public static string FilesFolder=>ServerConfig.Default.FilesFolder;
        public static string ImagesFolder => ServerConfig.Default.ImagesFolder;
    }
}
