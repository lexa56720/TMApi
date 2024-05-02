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
            //Если запрос пустой, то выход
            if (originalIds.Length == 0)
                return [];

            //Фильтр дубликатов
            var ids = originalIds.Distinct()
                                 .ToArray();

            //Получуение результата
            var apiResult = await apiFunc(request(ids));
            if (apiResult == null || apiResult.Items.Length == 0)
                return [];

            //Сопоставление запрашиваемых объектов и полученных
            var result = ids.Select(id => apiResult.Items.SingleOrDefault(i => selector(i) == id)).ToArray();

            //Если допустим возврат нулевых ссылок в массиве
            if (isNullValid || !result.Any(r => r == null))
                return result;
            return [];
        }
    }
}
