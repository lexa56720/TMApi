using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Friends;
using ApiTypes.Communication.Messages;
using ApiTypes.Shared;
using TMApi.ApiRequests.Chats;

namespace TMApi.ApiRequests.Messages
{
    public class Messages : BaseRequester
    {
        internal Messages(RequestSender requester, Api api) : base(requester, api)
        {
        }

        public async Task<Message[]> GetMessagesByOffset(int chatId, int count, int offset)
        {
            var messages = await Requester.ApiRequestAsync<MessageHistoryResponse, LastMessagesRequest>(new LastMessagesRequest(chatId, offset, count));
            if (messages == null)
                return [];

            return messages.Messages;
        }
        public async Task<Message[]> GetMessages(int chatId, int fromMessageId,int count)
        {
            var messages = await Requester.ApiRequestAsync<MessageHistoryResponse, MessageHistoryRequest>
                (new MessageHistoryRequest(chatId, fromMessageId,count));
            if (messages == null)
                return [];

            return messages.Messages;
        }
        public async Task<Message[]> GetMessages(params int[] messageIds)
        {
            return await RequestMany(messageIds,
                        (ids) => new MessageRequestById(ids),
                        Requester.ApiRequestAsync<SerializableArray<Message>, MessageRequestById>,
                        (x) => x.Id);
        }

        public async Task<Message?[]> GetMessagesForChats(params int[] chatIds)
        {
            var result = await RequestMany(chatIds,
                         (ids) => new MessageRequestByChats(ids),
                         Requester.ApiRequestAsync<SerializableArray<Message>, MessageRequestByChats>,
                         (x) => x.DestinationId,
                         true);
            if (result.Length > 0)
                return result;
            return Enumerable.Repeat<Message?>(null, chatIds.Length).ToArray();
        }

        public async Task<Message?> SendMessage(string text, int destinationId)
        {
            if (!DataConstraints.IsMessageLegal(text))
                return null;

            return await Requester.ApiRequestAsync<Message, MessageSendRequest>(new MessageSendRequest(text, destinationId));
        }

        public async Task<bool> MarkAsReaded(params int[] messageIds)
        {
            if (messageIds.Length < 0)
                return false;
            return await Requester.ApiSendAsync(new MarkAsReaded(messageIds));
        }
    }
}
