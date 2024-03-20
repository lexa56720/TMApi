using CSDTP;
using System.Diagnostics.CodeAnalysis;

namespace ApiTypes.Communication.Friends
{
    public class RequestResponse : ISerializable<RequestResponse>
    {
        public bool IsAccepted { get; set; }

        public int RequestId { get; set; } = -1;

        public RequestResponse(int requestId, bool isAccepted)
        {
            RequestId = requestId;
            IsAccepted = isAccepted;
        }
        public RequestResponse(bool isAccepted)
        {
            IsAccepted = isAccepted;
        }

        public RequestResponse()
        {
        }
    }
}
