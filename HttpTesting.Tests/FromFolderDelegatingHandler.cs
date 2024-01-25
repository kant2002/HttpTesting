using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpTesting.Tests
{

    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FromFolderDelegatingHandler : HttpMessageHandler
    {
        private readonly string folder;
        private readonly string captureFolder;

        public FromFolderDelegatingHandler(string folder, string captureFolder)
        {
            this.folder = folder;
            this.captureFolder = captureFolder;

            Directory.CreateDirectory(folder);
            Directory.CreateDirectory(captureFolder);
            foreach (var file in Directory.GetFiles(captureFolder))
            {
                File.Delete(file);
            }
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUri = request.RequestUri;
            var path = requestUri!.LocalPath;
            var method = request.Method;
            var baseName = method + "_" + path.Substring(1).Replace("/", "_");
            var fileToReturn = Path.Combine(this.folder, baseName + ".json");
            var fileToCapture = Path.Combine(this.captureFolder, baseName + ".json");
            if (File.Exists(fileToCapture))
            {
                fileToCapture = Path.Combine(this.captureFolder, baseName + "1" + ".json");
            }

            if (request.Content != null)
            {
                var requestContent = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                await File.WriteAllTextAsync(fileToCapture, requestContent, cancellationToken).ConfigureAwait(false);
            }
            else if (request.Method == HttpMethod.Post)
            {
                await File.WriteAllTextAsync(fileToCapture, string.Empty, cancellationToken).ConfigureAwait(false);
            }

            var content = await File.ReadAllTextAsync(fileToReturn, cancellationToken).ConfigureAwait(false);

            var responseContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = responseContent,
            };
            return response;
        }
    }
}
