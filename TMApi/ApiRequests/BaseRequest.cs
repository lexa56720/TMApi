using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.ApiRequests
{
    public abstract class BaseRequest
    {
        private protected RequestSender Requester { get; }
        internal BaseRequest(RequestSender requester)
        {
            Requester = requester;
        }
    }
}
