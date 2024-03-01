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

        public async Task<Message[]> GetMessages(int chatId, int count, int offset)
        {
            var messages = await Requester.ApiRequestAsync<MessageHistoryResponse, LastMessagesRequest>(new LastMessagesRequest(chatId, offset, count));
            if (messages == null)
                return [];

            return messages.Messages;
        }
        public async Task<Message[]> GetMessages(int chatId, int fromMessageId)
        {
            var messages = await Requester.ApiRequestAsync<MessageHistoryResponse, MessageHistoryRequest>(new MessageHistoryRequest(chatId, fromMessageId));
            if (messages == null)
                return [];

            return messages.Messages;
        }
        public async Task<Message[]> GetMessages(params int[] messagesId)
        {
            var messages = await Requester.ApiRequestAsync<SerializableArray<Message>, MessageRequest>(new MessageRequest(messagesId));
            if (messages == null)
                return [];

            return messages.Items;
        }
        public async Task<Message?> SendMessage(string text, int destinationId)
        {
            if (!DataConstraints.IsMessageLegal(text))
                return null;

            return await Requester.ApiRequestAsync<Message, MessageSendRequest>(new MessageSendRequest(text, destinationId));
        }
    }
}
