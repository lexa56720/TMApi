using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMApi.ApiRequests.Security;

namespace TMApi
{
    internal class AuthUpdater : IDisposable
    {
        private readonly Api Api;
        private Task? UpdateTask { get; set; }

        private CancellationTokenSource TokenSource = new();
        private bool isDisposed;

        public AuthUpdater(Api api)
        {
            Api = api;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                TokenSource.Dispose();
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }
        public async Task StartUpdate(TimeSpan delay)
        {
            if (UpdateTask != null && !UpdateTask.IsCompleted)
            {
                await TokenSource.CancelAsync();
                TokenSource.Dispose();
                TokenSource = new CancellationTokenSource();
            }
            try
            {
                UpdateTask = Task.Delay(delay, TokenSource.Token).ContinueWith(async (o) =>
                {
                    var updatedAuth = await Api.Auth.UpdateAuth();
                    if (updatedAuth != null)
                        Api.UpdateApiData(updatedAuth);
                }, TokenSource.Token);
            }
            catch (TaskCanceledException)
            {
            }
        }

    }
}
