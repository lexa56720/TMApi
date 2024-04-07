using ApiTypes.Communication.Medias;
using ApiTypes.Communication.Users;
using AutoSerializerSourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApiTypes.Communication.Messages
{
    public class MessageWithFilesSendRequest : BaseMessageSendRequest, ISerializable<MessageWithFilesSendRequest>
    {
        public BaseTypes.SerializableFile[] Files { get; set; } = [];

        public MessageWithFilesSendRequest(string text, int destinationId, BaseTypes.SerializableFile[] files)
        {
            Text = text;
            DestinationId = destinationId;
            Files = files;
        }

        public MessageWithFilesSendRequest()
        {

        }
    }
}
