using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Medias
{
    public enum ImageSize
    {
        Small,
        Medium,
        Large,
        Original,
    }
    public class PhotoLink : ISerializable<PhotoLink>
    {
        public string Url { get; set; } = string.Empty;

        public ImageSize Size { get; set; }
        public PhotoLink(string url, ImageSize size)
        {
            Url = url;
            Size = size;
        }

        public PhotoLink() { }
    }
}
