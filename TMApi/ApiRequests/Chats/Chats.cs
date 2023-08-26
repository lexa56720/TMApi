using ApiTypes.BaseTypes;
using ApiTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiTypes.Chats;
using ApiTypes.Shared;

namespace TMApi.ApiRequests.Chats
{
    public class Chats : BaseRequester
    {
        internal Chats(RequestSender requester, TMApi api) : base(requester, api)
        {
        }

        public async Task<bool> CreateChat(string name, int[] membersId)
        {
            return DataConstraints.IsNameLegal(name) && await Requester.GetRequestAsync
                (RequestHeaders.CreateChat, new ChatCreationRequest(name, membersId));
        }

        public async Task<Chat?> GetChat(int chatId)
        {
            return await Requester.PostRequestAsync<Chat, IntContainer>
                (RequestHeaders.GetChat, new IntContainer(chatId));
        }
        public async Task<Chat[]> GetChat(int[] chatIds)
        {
            var chats = await Requester.PostRequestAsync<SerializableArray<Chat>, IntArrayContainer>
                (RequestHeaders.GetChatMany, new IntArrayContainer(chatIds));
            if (chats == null)
                return Array.Empty<Chat>();
            return chats.Items;
        }

        public async Task<bool> SendChatInvite(int chatId, int toUserId)
        {
            return await Requester.GetRequestAsync
                (RequestHeaders.SendChatInvite, new ChatInvite(chatId, toUserId));
        }

        public async Task<ChatInvite?> GetChatInvite(int inviteId)
        {
            return await Requester.PostRequestAsync<ChatInvite, IntContainer>
                (RequestHeaders.GetChatInvite,new IntContainer(inviteId));
        }
        public async Task<ChatInvite[]> GetChatInvite(int[] inviteId)
        {
            var invites = await Requester
                .PostRequestAsync<SerializableArray<ChatInvite>, IntArrayContainer>
                (RequestHeaders.GetChatInviteMany, new IntArrayContainer(inviteId));

            if (invites == null)
                return Array.Empty<ChatInvite>();
            return invites.Items;
        }
    }
}
