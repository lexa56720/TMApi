using ApiTypes.BaseTypes;
using ApiTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.ApiRequests.Chats
{
    internal class Chats : BaseRequest
    {
        public Chats(RequestSender requester) : base(requester)
        {
        }

        public async Task<Chat> GetChat(int chatId)
        {
            return await Requester.PostRequestAsync<Chat, IntContainer>(new IntContainer(chatId));
        }


        public async Task<Chat[]> GetChats(int[] chatIds)
        {
            return (await Requester.PostRequestAsync<SerializableArray<Chat>, IntArrayContainer>(new IntArrayContainer(chatIds))).Items;
        }
    }
}
