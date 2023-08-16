using ApiTypes;
using CSDTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.User
{
    internal class Users
    {
        private RequestSender Requester { get; }
        public Users(RequestSender requester) 
        {
            Requester = requester;
        }

        public async Task<UserInfo> GetUserInfo(int userId)
        {
           await Requester.PostRequestAsync<>
        }

    }
}
