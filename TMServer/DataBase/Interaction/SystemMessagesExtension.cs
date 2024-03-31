using ApiTypes.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.DataBase.Tables;

namespace TMServer.DataBase.Interaction
{
    public static class SystemMessagesExtension
    {
        public static void AddInviteMessages(this Messages messages, int chatId, int inviterId, IEnumerable<int> targetIds, TmdbContext context)
        {
            foreach (var targetId in targetIds)
                messages.AddSystemMessage(chatId, inviterId, ActionKind.UserInvite, targetId, string.Empty, context);
        }
        public static void AddInviteMessages(this Messages messages, DBChat chat, int inviterId, IEnumerable<int> targetIds, TmdbContext context)
        {
            foreach (var targetId in targetIds)
                messages.AddSystemMessage(chat, inviterId, ActionKind.UserInvite, targetId, string.Empty, context);
        }
        public static void AddCreateMessage(this Messages messages, DBChat chat, int userId, TmdbContext context)
        {
            messages.AddSystemMessage(chat, userId, ActionKind.ChatCreated, null, string.Empty, context);
        }

        public static void AddEnterMessage(this Messages messages, int chatId, int userId, TmdbContext context)
        {
            messages.AddSystemMessage(chatId, userId, ActionKind.UserEnter, null, string.Empty, context);
        }
        public static void AddLeaveMessage(this Messages messages, int chatId, int userId, TmdbContext context)
        {
            messages.AddSystemMessage(chatId, userId, ActionKind.UserLeave, null, string.Empty, context);
        }
        public static void AddKickMessage(this Messages messages, int chatId, int userId, int kickId, TmdbContext context)
        {
            messages.AddSystemMessage(chatId, userId, ActionKind.UserKicked, kickId, string.Empty, context);
        }
        public static void AddRenameMessage(this Messages messages, int chatId, int userId, string newName, TmdbContext context)
        {
            messages.AddSystemMessage(chatId, userId, ActionKind.ChatRenamed, null, newName, context);
        }
        public static void AddUpdateCoverMessage(this Messages messages, int chatId, int userId, TmdbContext context)
        {
            messages.AddSystemMessage(chatId, userId, ActionKind.NewCover, null, string.Empty, context);
        }
    }
}
