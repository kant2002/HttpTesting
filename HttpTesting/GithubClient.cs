using RestSharp;

namespace HttpTesting;

public record GithubUser(int Id, string Login, string avatar_url, string Blog, string Location);

public class GithubClient: IDisposable
{
    readonly RestClient _client;

    public GithubClient(HttpMessageHandler? realHandler)
    {
        var options = new RestClientOptions("https://api.github.com/")
        {
            ConfigureMessageHandler = realHandler is null ? null : (_) => realHandler
        };

        _client = new RestClient(options);
    }

    public async Task<GithubUser> GetUser(string user)
    {
        var response = await _client.GetJsonAsync<GithubUser>(
            "users/{user}",
            new { user }
        );
        return response!;
    }

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}