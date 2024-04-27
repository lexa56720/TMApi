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

        public async Task<Chat?> CreateChat(ApiData<ChatCreationRequest> request)
        {
            if (!await Security.IsCanCreateChat(request.UserId, request.Data.Members))
                return null;

            var members = new List<int>(request.Data.Members);
            members.Insert(0, request.UserId); ;

            if (members.Count < 2 || !DataConstraints.IsNameLegal(request.Data.ChatName))
                return null;

            var chat = await Chats.CreateChat(request.Data.ChatName, members.ToArray());
            return await Converter.Convert(chat, 0);
        }

        public async Task<SerializableArray<Chat>?> GetChats(ApiData<ChatRequest> request)
        {
            if (!await Security.IsHaveAccessToChat(request.Data.Ids, request.UserId))
                return null;

            var chats = await Chats.GetChat(request.Data.Ids);
            if (chats == null || chats.Length == 0)
                return new SerializableArray<Chat>([]);

            var unreadCount = await Chats.GetUnreadCount(request.UserId, request.Data.Ids);
            return new SerializableArray<Chat>(await Converter.Convert(chats, unreadCount));
        }

        public async Task<SerializableArray<ChatInvite>> GetChatInvites(ApiData<InviteRequest> request)
        {
            var invites = await Chats.GetInvite(request.Data.Ids, request.UserId);
            if (invites.Length == 0)
                return new SerializableArray<ChatInvite>([]);

            return new SerializableArray<ChatInvite>(Converter.Convert(invites));
        }

        public async Task ChatInviteResponse(ApiData<ResponseToInvite> request)
        {
            if (!await Security.IsInviteExist(request.Data.InviteId, request.UserId))
                return;

            var invite = await Chats.RemoveInvite(request.Data.InviteId);
            if (!request.Data.IsAccepted)
                return;
            await Chats.AddUserToChat(request.UserId, invite.ChatId);
        }

        public async Task<IntArrayContainer?> GetAllChatInvites(ApiData<InviteRequestAll> request)
        {
            return new IntArrayContainer(await Chats.GetAllChatInvites(request.UserId));
        }

        public async Task AddChatInvite(ApiData<InviteToChatRequest> request)
        {
            if (!await Security.IsCanInviteToChat(request.UserId, request.Data.UserIds, request.Data.ChatId))
                return;

            await Chats.InviteToChat(request.UserId, request.Data.ChatId, request.Data.UserIds);
        }

        public async Task<IntArrayContainer?> GetAllChats(ApiData<ChatRequestAll> request)
        {
            var chats = await Chats.GetAllChats(request.UserId);
            if (chats.Length == 0)
                return new IntArrayContainer();
            return new IntArrayContainer(chats.Select(c => c.Id).ToArray());
        }

        internal async Task LeaveChat(ApiData<ChatLeaveRequest> request)
        {
            if (!await Security.IsMemberOfChat(request.UserId, request.Data.ChatId))
                return;

            await Chats.LeaveChat(request.UserId, request.Data.ChatId);
        }

        internal async Task ChangeName(ApiData<ChatChangeNameRequest> request)
        {
            if (!await Security.IsAdminOfChat(request.UserId, request.Data.ChatId) ||
                !DataConstraints.IsNameLegal(request.Data.NewName))
                return;
            await Chats.Rename(request.Data.ChatId, request.UserId, request.Data.NewName);
        }

        internal async Task KickUser(ApiData<ChatKickRequest> request)
        {
            if (!await Security.IsAdminOfChat(request.UserId, request.Data.ChatId) ||
                                        request.UserId == request.Data.UserId)
                return;
            await Chats.Kick(request.Data.ChatId, request.UserId, request.Data.UserId);
        }
    }
}
