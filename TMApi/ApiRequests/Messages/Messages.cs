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
            var messages = await Requester.PostAsync<MessageHistoryResponse, LastMessagesRequest>
                (RequestHeaders.GetMessageByOffset, new LastMessagesRequest(chatId, offset, count));

            if (messages == null)
                return Array.Empty<Message>();

            return messages.Messages;
        }
        public async Task<Message[]> GetMessages(int chatId, int fromMessageId)
        {
            var messages = await Requester.PostAsync<MessageHistoryResponse, MessageHistoryRequest>
                (RequestHeaders.GetMessagesByLastId, new MessageHistoryRequest(chatId, fromMessageId));

            if (messages == null)
                return Array.Empty<Message>();

            return messages.Messages;
        }
        public async Task<Message[]> GetMessages(params int[] messagesId)
        {
            var messages = await Requester.PostAsync<SerializableArray<Message>, IntArrayContainer>
                (RequestHeaders.GetMessagesById, new IntArrayContainer(messagesId));

            if (messages == null)
                return Array.Empty<Message>();

            return messages.Items;
        }
        public async Task<Message?> SendMessage(string text, int destinationId)
        {
            if (!DataConstraints.IsMessageLegal(text))
                return null;

            return await Requester.PostAsync< Message, MessageSendRequest>
                (RequestHeaders.NewMessage, new MessageSendRequest(text, destinationId));
        }
    }
}
