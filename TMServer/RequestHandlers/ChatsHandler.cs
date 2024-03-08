﻿using ApiTypes;
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

        public static SerializableArray<Chat> GetChats(ApiData<ChatRequest> request)
        {
            if (!request.Data.Ids.Select(v => Security.IsHaveAccessToChat(v, request.UserId)).All(a => a))
                return new SerializableArray<Chat>([]);

            var chats = Chats.GetChat(request.Data.Ids);
            if (chats.Length == 0)
                return new SerializableArray<Chat>([]);

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

        public static SerializableArray<ChatInvite> GetChatInvites(ApiData<InviteRequest> request)
        {
            var invites = Chats.GetInvite(request.Data.Ids, request.UserId);
            if (invites.Length == 0)
                return new SerializableArray<ChatInvite>([]);

            return new SerializableArray<ChatInvite>(
                invites.Select(i => new ChatInvite(
                                i.ChatId,
                                i.ToUserId,
                                i.InviterId)).ToArray());
        }

        public static void ChatInviteResponse(ApiData<RequestResponse> request)
        {
            if (Chats.GetInvite(request.Data.RequestId, request.UserId) != null)
                Chats.InviteResponse(request.Data.RequestId, request.UserId, request.Data.IsAccepted);
        }

        public static IntArrayContainer? GetAllChatInvites(ApiData<InviteRequestAll> request)
        {
            return new IntArrayContainer(Chats.GetAllChatInvites(request.UserId));
        }
        public static void SendChatInvite(ApiData<ChatInvite> request)
        {
            if (!Security.IsCanInviteToChat(request.UserId, request.Data.ToUserId, request.Data.ChatId))
                return;

            Chats.InviteToChat(request.UserId, request.Data.ToUserId, request.Data.ChatId);
        }

        public static IntArrayContainer? GetAllChats(ApiData<ChatRequestAll> request)
        {
            var chats = Chats.GetAllChats(request.UserId);
            if (chats.Length == 0)
                return null;
            return new IntArrayContainer(chats.Select(c => c.Id).ToArray());
        }
    }
}
