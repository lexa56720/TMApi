using ApiTypes;
using ApiTypes.Communication.Messages;
using ApiTypes.Shared;
using TMServer.DataBase;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    internal class MessagesHandler
    {
        public static void NewMessage(ApiData<MessageSendRequest> message)
        {
            if (DataConstraints.IsMessageLegal(message.Data.Text) &&
                Chats.IsHaveAccess(message.UserId, message.Data.DestinationId))
                Messages.AddMessage(message.UserId, message.Data.Text, message.Data.DestinationId);
        }

        public static MessageHistoryResponse? GetMessages(ApiData<MessageHistoryRequest> request)
        {
            if (!Chats.IsHaveAccess(request.UserId, request.Data.ChatId))
                return null;

            DBMessage[] dbMessages = Array.Empty<DBMessage>();
            if (request.Data.LastMessageId >= 0)
                dbMessages = Messages.GetMessages(request.Data.ChatId, request.Data.Offset,
                    request.Data.MaxCount, request.Data.LastMessageId, request.Data.LastMessageDate);
            else
                dbMessages = Messages.GetMessages(request.Data.ChatId, request.Data.Offset, request.Data.MaxCount);

            var messages = dbMessages.Select(m => new Message(m.Id, m.AuthorId, m.Content, m.SendTime));
            return new MessageHistoryResponse(request.Data.ChatId, messages.ToArray());
        }

    }
}
