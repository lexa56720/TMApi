using CSDTP.Cryptography.Providers;
using CSDTP.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMServer.Logger;
using TMServer.RequestHandlers;
using TMServer.ServerComponent.ApiResponser;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.Images
{
    internal class ImageServer : ResponseServer
    {
        private HttpListener Listener;
        private readonly ImageHandler ImageHandler;

        public ImageServer(int imageLoadPort, int imageGetPort,ImageHandler imageHandler ,IEncryptProvider encryptProvider, ILogger logger)
                           : base(imageLoadPort, encryptProvider, logger, Protocol.Http)
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add($"http://127.0.0.1:{imageGetPort}/");
            ImageHandler = imageHandler;
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            Listener.Close();
            base.Dispose();
        }

        public override void Start()
        {
            if (IsRunning)
                return;
            Listener.Start();
            base.Start();

            Listen();
        }

        public override void Stop()
        {
            base.Stop();
        }


        private Task Listen()
        {
            return Task.Run(async () =>
            {
                while (IsRunning)
                {
                    var context = await Listener.GetContextAsync();
                    if (TryParse(context.Request.RawUrl, out var url, out var id))
                    {
                        var image = ImageHandler.GetImage(url, id);
                        await GetResponse(image, context.Response);
                        await context.Response.OutputStream.FlushAsync();
                    }
                }
                Listener.Stop();
            });
        }
        private async Task GetResponse(byte[] imageData, HttpListenerResponse response)
        {
            if (imageData.Length == 0)
                response.StatusCode = 400;

            response.ContentLength64 = imageData.Length;
            response.ContentType = "image/jpeg";
            response.StatusCode = 200;

            using var ms = new MemoryStream(imageData);
            await ms.CopyToAsync(response.OutputStream);
        }
        private bool TryParse(string? rawUrl, out string url, out int id)
        {
            var urlParts = rawUrl.Split('/',StringSplitOptions.RemoveEmptyEntries);
            if (urlParts[0]=="images"&& int.TryParse(urlParts[2],out id))
            {
                url = urlParts[1];
                return true;
            }
            url = string.Empty;
            id = 0;
            return false;
        }
    }
}
