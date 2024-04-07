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
using TMServer.ServerComponent.Api;
using TMServer.ServerComponent.Basics;

namespace TMServer.ServerComponent.Files
{
    internal class FileServer : ApiServer
    {
        private HttpListener Listener;
        private readonly FileHandler FileHandler;

        public int DownloadPort { get; }
        public FileServer(FileHandler fileHandler, IEncryptProvider encryptProvider, ILogger logger)
                           : base(encryptProvider, logger, Protocol.Http)
        {
            Listener = new HttpListener();
            DownloadPort = CSDTP.Utils.PortUtils.GetFreePort(6666);
            Listener.Prefixes.Add($"http://127.0.0.1:{DownloadPort}/");
            FileHandler = fileHandler;
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

        private Task Listen()
        {
            return Task.Run(async () =>
            {
                while (IsRunning)
                {
                    var context = await Listener.GetContextAsync();
                    if (TryParse(context.Request.RawUrl, out var type, out var url, out var id))
                    {
                        switch (type)
                        {
                            case "images":
                                var image = await FileHandler.GetImageAsync(url, id);
                                await WriteResponse(image, context.Response, true);
                                break;
                            case "files":
                                var file = await FileHandler.GetFileAsync(url, id);
                                await WriteResponse(file, context.Response, true);
                                break;
                            default:
                                continue;
                        }
                        await context.Response.OutputStream.FlushAsync();
                    }
                }
                Listener.Stop();
            });
        }
        private async Task WriteResponse(byte[] imageData, HttpListenerResponse response, bool isImage)
        {
            if (imageData.Length == 0)
                response.StatusCode = 400;

            response.ContentLength64 = imageData.Length;
            response.ContentType = isImage ? "image/jpeg" : "application/x-binary";
            response.StatusCode = 200;

            using var ms = new MemoryStream(imageData);
            await ms.CopyToAsync(response.OutputStream);
        }
        private bool TryParse(string? rawUrl, out string type, out string url, out int id)
        {
            url = string.Empty;
            id = -1;
            type = "unknown";
            if (rawUrl == null)
                return false;

            var urlParts = rawUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (int.TryParse(urlParts[2], out id))
            {
                type = urlParts[0];
                url = urlParts[1];
                return true;
            }
            return false;
        }
    }
}
