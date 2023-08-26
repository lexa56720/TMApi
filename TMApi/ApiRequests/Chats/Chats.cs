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
            return DataConstraints.IsNameLegal(name) && await Requester.GetRequestAsync(new ChatCreationRequest(name, membersId));
        }

        public async Task<Chat?> GetChat(int chatId)
        {
            return await Requester.PostRequestAsync<Chat, IntContainer>(new IntContainer(chatId));
        }
        public async Task<Chat[]> GetChat(int[] chatIds)
        {
            var chats = await Requester.PostRequestAsync<SerializableArray<Chat>, IntArrayContainer>(new IntArrayContainer(chatIds));
            if (chats == null)
                return Array.Empty<Chat>();
            return chats.Items;
        }

        public async Task<bool> SendChatInvite(int chatId, int toUserId)
        {
            return await Requester.GetRequestAsync(new ChatInvite(chatId, toUserId));
        }

        public async Task<ChatInvite?> GetChatInvite(int inviteId)
        {
            return await Requester.PostRequestAsync<ChatInvite, IntContainer>(new IntContainer(inviteId));
        }
        public async Task<ChatInvite[]> GetChatInvite(int[] inviteId)
        {
            var invites = await Requester
                .PostRequestAsync<SerializableArray<ChatInvite>, IntArrayContainer>(new IntArrayContainer(inviteId));

            if (invites == null)
                return Array.Empty<ChatInvite>();
            return invites.Items;
        }
    }
}
