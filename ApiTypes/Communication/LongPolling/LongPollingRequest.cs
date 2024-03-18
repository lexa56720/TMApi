using CSDTP;

namespace ApiTypes.Communication.LongPolling
{
    public class LongPollingRequest : ISerializable<LongPollingRequest>
    {
        public int PreviousLongPollId { get; set; } = -1;

        public LongPollingRequest(int prevId)
        {
            PreviousLongPollId=prevId;
        }
        public LongPollingRequest()
        {
        }
    }
}
