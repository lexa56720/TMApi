using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTypes.Communication.Info
{
    public class ServerInfoResponse:ISerializable<ServerInfoResponse>
    {
        public int AuthPort { get; set; }
        public int ApiPort { get; set; }
        public int LongPollPort { get; set; }
        public int FileUploadPort { get; set; }
        public int FileGetPort { get; set; }
        public int LongPollPeriodSeconds { get; set; }
        public int Version { get; set; }
    }
}
