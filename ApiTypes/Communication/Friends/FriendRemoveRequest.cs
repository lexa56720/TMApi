using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Friends
{
    public class FriendRemoveRequest : ISerializable<FriendRemoveRequest>
    {
        public int FriendId { get; set; } = -1;

        public FriendRemoveRequest(int friendId)
        {
            FriendId = friendId;
        }

        public FriendRemoveRequest()
        {

        }
    }
}
