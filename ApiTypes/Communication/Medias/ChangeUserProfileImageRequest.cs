using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Medias
{
    public class ChangeUserProfileImageRequest:ISerializable<ChangeUserProfileImageRequest>
    {
        public byte[] ImageData { get; set; } = [];
        public ChangeUserProfileImageRequest(byte[] imageData)
        {
            ImageData = imageData;
        }
        public ChangeUserProfileImageRequest()
        {

        }
    }
}
