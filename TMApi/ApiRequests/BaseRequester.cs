using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.ApiRequests
{
    public abstract class BaseRequester
    {
        private protected RequestSender Requester { get; }
        internal BaseRequester(RequestSender requester)
        {
            Requester = requester;
        }
    }
}
