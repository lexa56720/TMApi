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
    internal class MessagesHandler
    {
        public static Message? NewMessage(ApiData<MessageSendRequest> request)
        {
            if (DataConstraints.IsMessageLegal(request.Data.Text) &&
                Security.IsMemberOfChat(request.UserId, request.Data.DestinationId))
            {
                var dbMessage = Messages.AddMessage(request.UserId, request.Data.Text, request.Data.DestinationId);
                var isReaded = Messages.IsMessageReaded(request.UserId, dbMessage.Id);
                Messages.ReadAllInChat(request.UserId, request.Data.DestinationId);
                return ConvertMessages(dbMessage, isReaded);
            }
            return null;
        }
        public static MessageHistoryResponse? GetMessagesByOffset(ApiData<LastMessagesRequest> request)
        {
            if (!Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = Messages.GetMessages(request.Data.ChatId, request.Data.Offset, request.Data.MaxCount);
            var isReaded = Messages.IsMessageReaded(request.UserId, dbMessages.Select(m => m.Id));
            return new MessageHistoryResponse()
            {
                FromId = request.Data.ChatId,
                Messages = ConvertMessages(dbMessages, isReaded)
            };
        }
        public static MessageHistoryResponse? GetMessagesByLastId(ApiData<MessageHistoryRequest> request)
        {
            if (!Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = Messages.GetMessages(request.Data.ChatId, 0, request.Data.MaxCount, request.Data.LastMessageId);

            var isReaded = Messages.IsMessageReaded(request.UserId, dbMessages.Select(m => m.Id));

            return new MessageHistoryResponse()
            {
                FromId = request.Data.ChatId,
                Messages = ConvertMessages(dbMessages, isReaded)
            };
        }
        public static SerializableArray<Message>? GetMessagesById(ApiData<MessagesRequest> request)
        {
            if (!Security.IsHaveAccessToMessages(request.UserId, request.Data.Ids))
                return null;

            var dbMessages = Messages.GetMessages(request.Data.Ids);
            var isReaded = Messages.IsMessageReaded(request.UserId, dbMessages.Select(m => m.Id));

            return new SerializableArray<Message>(ConvertMessages(dbMessages, isReaded));
        }
        private static Message ConvertMessages(DBMessage dbMessage, bool isReaded)
        {
            return new Message(dbMessage.Id, dbMessage.AuthorId, dbMessage.DestinationId,
                                             dbMessage.Content, dbMessage.SendTime, isReaded);
        }

        public static void MarkAsReaded(ApiData<MarkAsReaded> request)
        {
            if (Security.IsCanMarkAsReaded(request.UserId, request.Data.MessageIds))
                Messages.MarkAsReaded(request.UserId, request.Data.MessageIds);
        }

        private static Message[] ConvertMessages(DBMessage[] dbMessages, bool[] isReaded)
        {
            var result = new Message[dbMessages.Length];
            for (int i = 0; i < dbMessages.Length; i++)
                result[i] = ConvertMessages(dbMessages[i], isReaded[i]);

            return result;
        }
    }
}
