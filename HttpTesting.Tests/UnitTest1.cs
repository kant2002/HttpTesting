namespace HttpTesting.Tests
{
    [TestClass]
    public class UnitTest1
    {
        GithubClient CreateClient(string responsesFolder, string captureFolder)
        {
            var captureMode = false;
            if (captureMode)
                return new GithubClient(new ToFolderDelegatingHandler(responsesFolder) { InnerHandler = new HttpClientHandler() });

            // We do not capture HTTP headers and URL
            // If needed, then FromFolderDelegatingHandler should be augmented.
            // Our API was POST based mostly, so it was helpful enough.
            return new GithubClient(new FromFolderDelegatingHandler(responsesFolder, captureFolder));
        }

        [TestMethod]
        public async Task GetAnonymousUser()
        {
            // Arrange
            var testsCaseFolder = "test_cases/get_user_anon";
            var responsesFolder = Path.Combine(testsCaseFolder, "read");
            var captureFolder = Path.Combine(testsCaseFolder, "write");
            var expectedFolder = Path.Combine(testsCaseFolder, "expected");
            var client = CreateClient(responsesFolder, captureFolder);

            // Act
            var user = await client.GetUser("kant2002");

            // Assert
            Assert.AreEqual(user.Login, "kant2002");
            Assert.AreEqual(user.Id, 4257079);
            Assert.AreEqual(user.avatar_url, "https://avatars.githubusercontent.com/u/4257079?v=4");
            Assert.AreEqual(user.Blog, "https://codevision.medium.com/");
            Assert.AreEqual(user.Location, "Almaty, Kazakhstan");
            AssertDirectoriesContentSame(expectedFolder, captureFolder);
        }
        
        // We can use Verify here.
        private static void AssertDirectoriesContentSame(string expectedDirectory, string actualDirectory)
        {
            var filesToExclude = Directory.GetFiles(expectedDirectory, "*.gitignore");
            var expectedFiles = Directory.GetFiles(expectedDirectory)
                .Except(filesToExclude)
                .OrderBy(_ => _)
                .ToArray();
            var actualFiles = Directory.GetFiles(actualDirectory)
                .OrderBy(_ => _)
                .ToArray();
            var expectedFileNames = expectedFiles
                .Select(_ => _.Replace(expectedDirectory, string.Empty))
                .ToArray();
            var actualFileNames = actualFiles
                .Select(_ => _.Replace(actualDirectory, string.Empty))
                .ToArray();
            CollectionAssert.AreEquivalent(expectedFileNames, actualFileNames);

            for (var i = 0; i < expectedFiles.Length; i++)
            {
                AssertFilesContentSame(expectedFiles[i], actualFiles[i]);
            }
        }

        private static void AssertFilesContentSame(string expectedFilePath, string actualFilePath)
        {
            var expectedContent = File.ReadAllText(expectedFilePath);
            var actualContent = File.ReadAllText(actualFilePath);

            Assert.AreEqual(expectedContent, actualContent);
        }
    }
}