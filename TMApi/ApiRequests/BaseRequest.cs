using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMApi.ApiRequests
{
    internal abstract class BaseRequest
    {
        protected RequestSender Requester { get; }
        public BaseRequest(RequestSender requester)
        {
            Requester = requester;
        }
    }
}
