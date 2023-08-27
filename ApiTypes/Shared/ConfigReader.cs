using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Shared
{
    public static class ConfigReader
    {
        public static Dictionary<string, string> Read(string path, bool isEmbedded)
        {
            if (!isEmbedded)
                return Read(path);

            var assembly = Assembly.GetEntryAssembly();
            var res = assembly.GetManifestResourceNames();
            using var stream = assembly.GetManifestResourceStream(res.First(r => r.Contains(path)));
            return Read(stream);
        }

        private static Dictionary<string, string> Read(string path)
        {
            using var stream = File.OpenRead(path);
            return Read(stream);
        }
        public static Dictionary<string, string> Read(Stream? stream)
        {
            var dictionary = new Dictionary<string, string>();

            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var parsedLine = ParseLine(line);
                    dictionary.TryAdd(parsedLine.Item1, parsedLine.Item2);
                }
            }
            return dictionary;
        }
        private static (string, string) ParseLine(string line)
        {
            var keyValue = line.Split('=', 2, StringSplitOptions.TrimEntries);
            return (keyValue[0], keyValue[1]);
        }
    }
}
