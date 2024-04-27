using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Messages;
using ApiTypes.Shared;
using CSDTP.Requests;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    public class MessagesHandler
    {
        private readonly Security Security;
        private readonly Messages Messages;
        private readonly DbConverter Converter;

        public MessagesHandler(Security security, Messages messages, DbConverter converter)
        {
            Security = security;
            Messages = messages;
            Converter = converter;
        }
        public async Task<Message?> NewMessage(ApiData<MessageSendRequest> request)
        {
            if (!DataConstraints.IsMessageLegal(request.Data.Text) ||
                !await Security.IsMemberOfChat(request.UserId, request.Data.DestinationId))
                return null;

            var dbMessage = await Messages.AddMessage(request.UserId, request.Data.Text, request.Data.DestinationId);
            await Messages.AddToUnread(dbMessage.Id, dbMessage.DestinationId);
            var isReaded = await Messages.IsMessageReaded(request.UserId, dbMessage.Id);

            await Messages.ReadAllInChat(request.UserId, request.Data.DestinationId);
            return await Converter.Convert(dbMessage, isReaded);
        }
        public async Task<SerializableArray<Message>?> GetMessagesByOffset(ApiData<LastMessagesRequest> request)
        {
            if (!await Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = await Messages.GetMessages(request.Data.ChatId, request.Data.Offset, request.Data.MaxCount);
            var isReaded = await Messages.IsMessageReaded(request.UserId, dbMessages.Select(m => m.Id));
            return new SerializableArray<Message>()
            {
                Items = await Converter.Convert(dbMessages, isReaded)
            };
        }
        public async Task<SerializableArray<Message>?> GetMessagesByLastId(ApiData<MessageHistoryRequest> request)
        {
            if (!await Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = await Messages.GetMessages(request.Data.ChatId, 0, request.Data.MaxCount, request.Data.LastMessageId);

            var isReaded = await Messages.IsMessageReaded(request.UserId, dbMessages.Select(m => m.Id));
            return new SerializableArray<Message>()
            {
                Items = await Converter.Convert(dbMessages, isReaded)
            };
        }
        public async Task<SerializableArray<Message>?> GetMessagesById(ApiData<MessageRequestById> request)
        {
            if (!await Security.IsHaveAccessToMessages(request.UserId, request.Data.Ids))
                return null;

            var dbMessages = await Messages.GetMessages(request.Data.Ids);
            var isReaded = await Messages.IsMessageReaded(request.UserId, dbMessages.Select(m => m.Id));

            return new SerializableArray<Message>(await Converter.Convert(dbMessages, isReaded));
        }

        public async Task<SerializableArray<Message>?> GetMessagesByChatIds(ApiData<MessageRequestByChats> request)
        {
            if (!await Security.IsHaveAccessToChat(request.Data.Ids, request.UserId))
                return null;
            var dbMessages = await Messages.GetLastMessages(request.Data.Ids);
            var isReaded = await Messages.IsMessageReaded(request.UserId, dbMessages.Select(m => m.Id));

            return new SerializableArray<Message>(await Converter.Convert(dbMessages, isReaded));
        }

        public async Task MarkAsReaded(ApiData<MarkAsReaded> request)
        {
            if (await Security.IsCanMarkAsReaded(request.UserId, request.Data.MessageIds))
                await Messages.MarkAsReaded(request.UserId, request.Data.MessageIds);
        }

    }
}
