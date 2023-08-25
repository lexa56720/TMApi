using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.DataBase.Tables
{
    public class DBFriendRequest
    {
        public int Id { get; set; }
        public required int UserIdOne { get; set; }
        public required int UserIdTwo { get; set; }

        public DBUser UserOne { get; set; } = null!;

        public DBUser UserTwo { get; set; } = null!;
    }
}
