using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.BaseTypes
{
    public class RequestResponse : ISerializable<RequestResponse>
    {
        public bool IsAccepted { get; set; }

        public int RequestId { get; set; } = -1;


        [SetsRequiredMembers]
        public RequestResponse(int requestId, bool isAccepted)
        {
            RequestId = requestId;
            IsAccepted = isAccepted;
        }
        [SetsRequiredMembers]
        public RequestResponse(bool isAccepted)
        {
            IsAccepted = isAccepted;
        }

        public RequestResponse()
        {
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(IsAccepted);
            writer.Write(RequestId);
        }

        public static RequestResponse Deserialize(BinaryReader reader)
        {
            return new RequestResponse()
            {
                IsAccepted = reader.ReadBoolean(),
                RequestId = reader.ReadInt32(),
            };
        }
    }
}
