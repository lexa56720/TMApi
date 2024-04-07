using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Medias
{
    public class FileLink : ISerializable<FileLink>
    {
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public FileLink(string url,string name)
        {
            Url = url;
            Name = name;
        }

        public FileLink() { }
    }
}
