using ApiTypes;
using ApiTypes.BaseTypes;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.ApiRequests.Users
{
    internal class Users:BaseRequest
    {
        public Users(RequestSender requester):base(requester) 
        {
        }

        public async Task<UserInfo> GetUserInfo(int userId)
        {
            return await Requester.PostRequestAsync<UserInfo, IntContainer>(new IntContainer(userId));
        }
        public async Task<User> GetUser(int userId)
        {
            return await Requester.PostRequestAsync<User, IntContainer>(new IntContainer(userId));
        }

        public async Task<User[]> GetUser(int[] userId)
        {
            return (await Requester.PostRequestAsync<SerializableArray<User>, IntArrayContainer>(new IntArrayContainer(userId))).Items;
        }
    }
}
