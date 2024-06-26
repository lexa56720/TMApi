﻿using ApiTypes.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMServer
{
    internal static class Settings
    {
        public static int Version => ServerConfig.Default.Version;
        public static int InfoPort => ServerConfig.Default.InfoPort;
        public static int AuthPort => ServerConfig.Default.AuthPort;
        public static int ApiPort=> ServerConfig.Default.ApiPort;
        public static int LongPollPort => ServerConfig.Default.LongPollPort;
        public static int FileDownloadPort => ServerConfig.Default.FileDownloadPort;
        public static int FileUploadPort => ServerConfig.Default.FileUploadPort;


        public static int ExternalInfoPort => ServerConfig.Default.ExternalInfoPort;
        public static int ExternalAuthPort => ServerConfig.Default.ExternalAuthPort;
        public static int ExternalApiPort => ServerConfig.Default.ExternalApiPort;
        public static int ExternalLongPollPort => ServerConfig.Default.ExternalLongPollPort;
        public static int ExternalFileDownloadPort => ServerConfig.Default.ExternalFileDownloadPort;
        public static int ExternalFileUploadPort => ServerConfig.Default.ExternalFileUploadPort;

        public static TimeSpan LongPollLifeTime => TimeSpan.FromSeconds(ServerConfig.Default.LongPollLifeTimeSeconds);
        public static TimeSpan TokenLifeTime => TimeSpan.FromHours(ServerConfig.Default.TokenLifetimeHours);
        public static TimeSpan RsaLifeTime => TimeSpan.FromHours(ServerConfig.Default.RsaKeyLifetimeHours);
        public static TimeSpan AesLifeTime => TimeSpan.FromDays(ServerConfig.Default.AesKeyLifetimeDays);

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
