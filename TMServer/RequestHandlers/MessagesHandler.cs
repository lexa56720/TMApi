using ApiTypes;
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
                Chats.IsMemberOfChat(message.UserId, message.Data.DestinationId))
            {
                var dbMessage = Messages.AddMessage(message.UserId, message.Data.Text, message.Data.DestinationId);

                return new Message(dbMessage.Id, dbMessage.AuthorId, dbMessage.Content, dbMessage.SendTime);
            }
            return null;
        }
        public static MessageHistoryResponse? GetMessagesByOffset(ApiData<LastMessagesRequest> request)
        {
            if (!Chats.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = Messages.GetMessages
                             (request.Data.ChatId, request.Data.Offset, request.Data.MaxCount);
            return new MessageHistoryResponse()
            {
                FromId = request.Data.ChatId,
                Messages = dbMessages.Select(ConvertMessages).ToArray()
            };
        }
        public static MessageHistoryResponse? GetMessagesById(ApiData<MessageHistoryRequest> request)
        {
            if (!Chats.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            var dbMessages = Messages.GetMessages
                             (request.Data.ChatId,0,request.Data.MaxCount,request.Data.LastMessageId);
            return new MessageHistoryResponse()
            {
               FromId = request.Data.ChatId,
               Messages= dbMessages.Select(ConvertMessages).ToArray()
            };
        }

        private static Message ConvertMessages(DBMessage dBMessage)
        {
            return new Message(dBMessage.Id, dBMessage.AuthorId, dBMessage.Content, dBMessage.SendTime);
        }
    }
}
