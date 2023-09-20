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

        public static MessageHistoryResponse? GetMessages(ApiData<MessageHistoryRequest> request)
        {
            if (!Chats.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return null;

            DBMessage[] dbMessages;
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
