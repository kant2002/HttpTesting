namespace HttpTesting.Tests;

using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

internal class ToFolderDelegatingHandler : DelegatingHandler
{
    private readonly string folder;
    private int counter = 1;

    public ToFolderDelegatingHandler(string folder)
    {
        this.folder = folder;
        Directory.CreateDirectory(folder);
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var fileToReturn = Path.Combine(this.folder, (this.counter++) + ".json");
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        await File.WriteAllTextAsync(fileToReturn, content, cancellationToken).ConfigureAwait(false);
        return response;
    }
}