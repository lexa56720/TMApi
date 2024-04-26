using ApiTypes.Communication.BaseTypes;
using TMApi.API;

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

        protected async Task<T[]> RequestMany<T, U>(int[] originalIds, Func<int[], U> request,
            Func<U, Task<SerializableArray<T>?>> apiFunc, Func<T, int> selector, bool isNullValid = false)
                                  where T : ISerializable<T>, new()
        {
            if (originalIds.Length == 0)
                return [];

            var ids = originalIds.Distinct()
                                 .ToArray();
            var apiResult = await apiFunc(request(ids));
            if (apiResult == null || apiResult.Items.Length == 0)
                return [];

            var result = ids.Select(id => apiResult.Items.SingleOrDefault(i => selector(i) == id)).ToArray();

            if (isNullValid)
                return result;

            if (result.Any(r => r == null))
                return [];
            return result;
        }
    }
}
