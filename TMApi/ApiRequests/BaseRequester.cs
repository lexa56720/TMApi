namespace TMApi.ApiRequests
{
    public abstract class BaseRequester : IDisposable
    {
        private protected RequestSender Requester { get; }
        protected TMApi Api { get; }

        internal BaseRequester(RequestSender requester, TMApi api)
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
