using ApiTypes.BaseTypes;
using ApiTypes;
using ApiTypes.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMApi.ApiRequests.Chats;

namespace TMApi.ApiRequests.Messages
{
    internal class Messages : BaseRequest
    {
        public Messages(RequestSender requester) : base(requester)
        {
        }

        public async Task<Message[]> GetLastMessages(int chatId,int count)
        {
            return (await Requester.PostRequestAsync<SerializableArray<Message>, IntArrayContainer>(new IntArrayContainer(chatId,count))).Items;
        }
    }
}
