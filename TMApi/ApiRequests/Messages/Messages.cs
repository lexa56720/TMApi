﻿using ApiTypes;
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
        public async Task<Message[]> GetMessages(params int[] messageIds)
        {
            var messages = await Requester.ApiRequestAsync<SerializableArray<Message>, MessageRequestById>(new MessageRequestById(messageIds));
            if (messages == null)
                return [];

            return messages.Items;
        }

        public async Task<Message[]> GetMessagesForChats(params int[] chatIds)
        {
            var messages = await Requester.ApiRequestAsync
                <SerializableArray<Message>, MessageRequestByChats>(new MessageRequestByChats(chatIds));
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

        public async Task<bool> MarkAsReaded(params int[] messageIds)
        {
            return await Requester.ApiSendAsync(new MarkAsReaded(messageIds));
        }
    }
}
