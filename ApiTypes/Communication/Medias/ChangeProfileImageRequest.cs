using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Medias
{
    public class ChangeProfileImageRequest:ISerializable<ChangeProfileImageRequest>
    {
        public byte[] ImageData { get; set; } = [];
        public ChangeProfileImageRequest(byte[] imageData)
        {
            ImageData = imageData;
        }
        public ChangeProfileImageRequest()
        {

        }
    }
}
