using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Messages;
using ApiTypes.Shared;
using TMServer.DataBase;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    internal class MessagesHandler
    {
        public static Message? NewMessage(ApiData<MessageSendRequest> message)
        {
            if (DataConstraints.IsMessageLegal(message.Data.Text) &&
               Security.IsMemberOfChat(message.UserId, message.Data.DestinationId))
            {
                var dbMessage = Messages.AddMessage(message.UserId, message.Data.Text, message.Data.DestinationId);

                return ConvertMessages(dbMessage);
            }
            return null;
        }
        public static MessageHistoryResponse? GetMessagesByOffset(ApiData<LastMessagesRequest> request)
        {
            if (!Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = Messages.GetMessages
                             (request.Data.ChatId, request.Data.Offset, request.Data.MaxCount);
            return new MessageHistoryResponse()
            {
                FromId = request.Data.ChatId,
                Messages = dbMessages.Select(ConvertMessages).ToArray()
            };
        }
        public static MessageHistoryResponse? GetMessagesByLastId(ApiData<MessageHistoryRequest> request)
        {
            if (!Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = Messages.GetMessages
                             (request.Data.ChatId, 0, request.Data.MaxCount, request.Data.LastMessageId);
            return new MessageHistoryResponse()
            {
                FromId = request.Data.ChatId,
                Messages = dbMessages.Select(ConvertMessages).ToArray()
            };
        }
        public static SerializableArray<Message>? GetMessagesById(ApiData<IntArrayContainer> request)
        {
            var dbMessages = Messages.GetMessages(request.Data.Values);

            if (dbMessages.Any(m => !Security.IsMemberOfChat(request.UserId, m.DestinationId)))
                return null;

            return new SerializableArray<Message>(dbMessages.Select(ConvertMessages).ToArray());
        }
        private static Message ConvertMessages(DBMessage dbMessage)
        {
            return new Message(dbMessage.Id, dbMessage.AuthorId, 
                               dbMessage.DestinationId, dbMessage.Content, dbMessage.SendTime);
        }
    }
}
