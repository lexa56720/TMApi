using ApiTypes;
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
            var messages = await Requester.PostRequestAsync<MessageHistoryResponse, MessageHistoryRequest>
                (RequestHeaders.GetLastMessages, new MessageHistoryRequest(chatId, offset, count));

            if (messages == null)
                return Array.Empty<Message>();

            return messages.Messages;
        }

        public async Task<bool> SendMessage(string text, int destinationId)
        {
            if (!DataConstraints.IsMessageLegal(text))
                return false;

            return await Requester.GetRequestAsync
                (RequestHeaders.SendMessage, new MessageSendRequest(text, destinationId));
        }

    }
}
