using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Communication.Messages;
using ApiTypes.Communication.Users;
using ApiTypes.Shared;
using CSDTP.Requests;
using TMServer.DataBase.Interaction;
using TMServer.DataBase.Tables;

namespace TMServer.RequestHandlers
{
    public class ChatsHandler
    {
        private readonly Security Security;
        private readonly Chats Chats;
        private readonly DbConverter Converter;

        public ChatsHandler(Security security, Chats chats, DbConverter converter)
        {
            Security = security;
            Chats = chats;
            Converter = converter;
        }

        public Chat? CreateChat(ApiData<ChatCreationRequest> request)
        {
            if (!Security.IsCanCreateChat(request.UserId, request.Data.Members))
                return null;

            var members = new List<int>(request.Data.Members);
            members.Insert(0, request.UserId); ;

            if (members.Count < 2 || !DataConstraints.IsNameLegal(request.Data.ChatName))
                return null;

            var chat = Chats.CreateChat(request.Data.ChatName, members.ToArray());
            return Converter.Convert(chat, 0);
        }

        public SerializableArray<Chat>? GetChats(ApiData<ChatRequest> request)
        {
            if (!Security.IsHaveAccessToChat(request.Data.Ids, request.UserId))
                return null;

            var chats = Chats.GetChat(request.Data.Ids);
            if (chats.Length == 0)
                return new SerializableArray<Chat>([]);

            var unreadCount = Chats.GetUnreadCount(request.UserId, request.Data.Ids);
            return new SerializableArray<Chat>(Converter.Convert(chats, unreadCount));
        }

        public SerializableArray<ChatInvite> GetChatInvites(ApiData<InviteRequest> request)
        {
            var invites = Chats.GetInvite(request.Data.Ids, request.UserId);
            if (invites.Length == 0)
                return new SerializableArray<ChatInvite>([]);

            return new SerializableArray<ChatInvite>(Converter.Convert(invites));
        }

        public void ChatInviteResponse(ApiData<ResponseToInvite> request)
        {
            if (!Security.IsInviteExist(request.Data.InviteId, request.UserId))
                return;

            var invite = Chats.RemoveInvite(request.Data.InviteId);
            if (!request.Data.IsAccepted)
                return;
            Chats.AddUserToChat(request.UserId, invite.ChatId);
        }

        public IntArrayContainer? GetAllChatInvites(ApiData<InviteRequestAll> request)
        {
            return new IntArrayContainer(Chats.GetAllChatInvites(request.UserId));
        }

        public void RegisterChatInvite(ApiData<InviteToChatRequest> request)
        {
            if (!Security.IsCanInviteToChat(request.UserId, request.Data.UserIds, request.Data.ChatId))
                return;

            Chats.InviteToChat(request.UserId, request.Data.ChatId, request.Data.UserIds);
        }

        public IntArrayContainer? GetAllChats(ApiData<ChatRequestAll> request)
        {
            var chats = Chats.GetAllChats(request.UserId);
            if (chats.Length == 0)
                return new IntArrayContainer();
            return new IntArrayContainer(chats.Select(c => c.Id).ToArray());
        }

        internal void LeaveChat(ApiData<ChatLeaveRequest> request)
        {
            if (!Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return;

            Chats.LeaveChat(request.UserId, request.Data.ChatId);
        }

        internal void ChangeName(ApiData<ChatChangeNameRequest> request)
        {
            if (!Security.IsAdminOfChat(request.UserId, request.Data.ChatId) ||
                !DataConstraints.IsNameLegal(request.Data.NewName))
                return;
            Chats.Rename(request.Data.ChatId, request.Data.NewName);
        }

        internal void KickUser(ApiData<ChatKickRequest> request)
        {
            if (!Security.IsAdminOfChat(request.UserId, request.Data.ChatId))
                return;
            Chats.Kick(request.Data.ChatId, request.UserId, request.Data.UserId);
        }
    }
}
