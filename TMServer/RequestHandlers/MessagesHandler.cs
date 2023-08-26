using ApiTypes;
using ApiTypes.Messages;
using ApiTypes.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase;

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
            if (!Chats.IsHaveAccess(request.UserId, request.Data.FromId))
                return null;

            var dbMessages = Messages.GetMessages(request.Data.FromId, request.Data.Offset, request.Data.MaxCount);
            var messages = dbMessages.Select(m => new Message(m.Id, m.AuthorId, m.Content, m.SendTime));
            return new MessageHistoryResponse(request.Data.FromId, messages.ToArray());
        }

    }
}
