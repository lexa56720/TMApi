using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Users
{
    public class FriendResponse : ISerializable<FriendResponse>
    {
        public required bool IsAccepted { get; init; }

        public required FriendRequest Request { get; init; }


        FriendResponse(bool isAccepted, FriendRequest request)
        {
            IsAccepted = isAccepted;
            Request = request;
        }
        FriendResponse()
        {

        }

        public static FriendResponse Deserialize(BinaryReader reader)
        {
            return new FriendResponse()
            {
                IsAccepted = reader.ReadBoolean(),
                Request = FriendRequest.Deserialize(reader)
            };
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsAccepted);
            Request.Serialize(writer);
        }
    }
}
