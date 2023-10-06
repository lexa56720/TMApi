using ApiTypes;
using ApiTypes.Communication.BaseTypes;
using ApiTypes.Communication.Chats;
using ApiTypes.Shared;
using TMServer.DataBase.Interaction;

namespace TMServer.RequestHandlers
{
    internal class ChatsHandler
    {
        public static Chat? CreateChat(ApiData<ChatCreationRequest> request)
        {
            if (!Security.IsCanCreateChat(request.UserId, request.Data.Members))
                return null;

            var members = new List<int>(request.Data.Members);
            members.Insert(0, request.UserId);
            var chatMembers = members.Distinct().ToArray();

            if (chatMembers.Length < 2 || !DataConstraints.IsNameLegal(request.Data.ChatName))
                return null;

            var chat = Chats.CreateChat(request.Data.ChatName, false, chatMembers);
            return new Chat()
            {
                Id = chat.Id,
                AdminId = chat.Admin.Id,
                Name = request.Data.ChatName,
                MemberIds = chat.Members.Select(m => m.Id).ToArray(),
                IsDialogue = chat.IsDialogue,
            };
        }

        public static Chat? GetChat(ApiData<IntContainer> request)
        {
            if (!Security.IsHaveAccessToChat(request.Data.Value, request.UserId))
                return null;

            var chat = Chats.GetChat(request.Data.Value);
            if (chat == null)
                return null;

            return new Chat()
            {
                Id = chat.Id,
                AdminId = chat.Admin.Id,
                Name = chat.Name,
                MemberIds = chat.Members.Select(m => m.Id).ToArray(),
                IsDialogue = chat.IsDialogue,
            };
        }
        public static SerializableArray<Chat> GetChats(ApiData<IntArrayContainer> request)
        {
            if (!request.Data.Values.Select(v => Security.IsHaveAccessToChat(v, request.UserId)).All(a => a))
                return new SerializableArray<Chat>(Array.Empty<Chat>());

            var chats = Chats.GetChat(request.Data.Values);
            if (!chats.Any())
                return new SerializableArray<Chat>(Array.Empty<Chat>());

            return new SerializableArray<Chat>(
                chats.Select(c => new Chat()
                {
                    Id = c.Id,
                    AdminId = c.Admin.Id,
                    Name = c.Name,
                    MemberIds = c.Members.Select(m => m.Id).ToArray(),
                    IsDialogue = c.IsDialogue,
                }).ToArray());
        }

        public static void SendChatInvite(ApiData<ChatInvite> request)
        {
            if (!Security.IsCanInviteToChat(request.UserId, request.Data.ToUserId, request.Data.ChatId))
                return;

            Chats.InviteToChat(request.UserId, request.Data.ToUserId, request.Data.ChatId);
        }
        public static ChatInvite? GetChatInvite(ApiData<IntContainer> request)
        {
            var invite = Chats.GetInvite(request.Data.Value, request.UserId);
            if (invite == null)
                return null;
            return new ChatInvite(invite.ChatId, invite.ToUserId, invite.InviterId, invite.Id);

        }
        public static SerializableArray<ChatInvite> GetChatInvites(ApiData<IntArrayContainer> request)
        {
            var invites = Chats.GetInvite(request.Data.Values, request.UserId);
            if (!invites.Any())
                return new SerializableArray<ChatInvite>(Array.Empty<ChatInvite>());

            return new SerializableArray<ChatInvite>(
                invites.Select(i =>
                    new ChatInvite(
                        i.ChatId,
                        i.ToUserId,
                        i.InviterId)
                ).ToArray()
            );
        }

        public static void ChatInviteResponse(ApiData<RequestResponse> request)
        {
            if (Chats.GetInvite(request.Data.RequestId, request.UserId) != null)
                Chats.InviteResponse(request.Data.RequestId, request.UserId, request.Data.IsAccepted);
        }

        public static IntArrayContainer? GetAllChatInvites(ApiData<IntContainer> userId)
        {
            if (userId.UserId != userId.Data.Value)
                return null;

            return new IntArrayContainer(Chats.GetAllChatInvites(userId.UserId));
        }


        public static SerializableArray<Chat> GetAllByDialogue(ApiData<ChatRequest> request)
        {
            var chats = Chats.GetAllChatByDialogue(request.UserId, request.Data.IsDialogues);

            return new SerializableArray<Chat>(chats.Select(c => new Chat()
            {
                Id = c.Id,
                AdminId = c.AdminId,
                MemberIds = c.Members.Select(m => m.Id).ToArray(),
                IsDialogue = c.IsDialogue,
                Name = c.Name,
            }).ToArray());
        }

    }
}
