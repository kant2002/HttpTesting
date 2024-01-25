using HttpTesting;

var client = new GithubClient(null);
var kant2002 = await client.GetUser("kant2002");

Console.WriteLine($"See, it's me: {kant2002.avatar_url}");
Console.WriteLine($"My blog: {kant2002.Blog}");
Console.WriteLine($"I live in {kant2002.Location}");
