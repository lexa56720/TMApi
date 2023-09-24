using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Messages;
using ApiTypes.Shared;

namespace TMApi.ApiRequests.Messages
{
    public class Messages : BaseRequester
    {
        internal Messages(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public async Task<Message[]?> GetLastMessages(int chatId, int count, int offset)
        {
            var messages = await Requester.PostAsync<MessageHistoryResponse, MessageHistoryRequest>
                (RequestHeaders.GetLastMessages, new MessageHistoryRequest(chatId, offset, count));

            if (messages == null)
                return Array.Empty<Message>();

            return messages.Messages;
        }

        public async Task<Message?> SendMessage(string text, int destinationId)
        {
            if (!DataConstraints.IsMessageLegal(text))
                return null;

            return await Requester.PostAsync< Message, MessageSendRequest>
                (RequestHeaders.SendMessage, new MessageSendRequest(text, destinationId));
        }

    }
}
