namespace TMApi.ApiRequests
{
    public abstract class BaseRequester : IDisposable
    {
        private protected RequestSender Requester { get; }
        protected Api Api { get; }

        internal BaseRequester(RequestSender requester, Api api)
        {
            Requester = requester;
            Api = api;
        }

        public virtual void Dispose()
        {
            Requester.Dispose();
        }
    }
}
