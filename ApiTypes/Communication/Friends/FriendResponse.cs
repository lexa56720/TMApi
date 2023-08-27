using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Friends
{
    public class FriendResponse : ISerializable<FriendResponse>
    {
        public bool IsAccepted { get; init; }

        public FriendRequest Request { get; init; }


        [SetsRequiredMembers]
        public FriendResponse(FriendRequest request, bool isAccepted)
        {
            Request = request;
            IsAccepted = isAccepted;
        }

        public FriendResponse()
        {
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsAccepted);
            Request.Serialize(writer);
        }

        public static FriendResponse Deserialize(BinaryReader reader)
        {
            return new FriendResponse()
            {
                IsAccepted = reader.ReadBoolean(),
                Request = FriendRequest.Deserialize(reader),
            };
        }
    }
}
