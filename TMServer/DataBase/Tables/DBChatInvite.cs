using ApiTypes.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public class DBChatInvite
    {
        public int Id { get; set; }

        public required int ChatId { get; set; }

        public required int InviterId { get; set; }

        public required int UserId { get; set; }


        public DBChat Chat { get; set; } = null!;

        public DBUser Inviter { get; set; } = null!;

        public DBUser DestinationUser { get; set; } = null!;

    }
}
