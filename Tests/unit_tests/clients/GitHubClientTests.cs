using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHubReleasesCLI.clients;
using GitHubReleasesCLI.dtos;

namespace Tests.unit_tests.clients
{
    public class GitHubClientTests
    {
        private GitHubClient gitHubClient;
        private Mock<BaseHttpClient> httpClient;

        public GitHubClientTests()
        {
            string gitHubToken = "token";
            string repoOwner = "owner";
            httpClient = new Mock<BaseHttpClient>();

            gitHubClient = new GitHubClient(gitHubToken, repoOwner, httpClient.Object);
        }

        [Fact]
        public async Task CreateRelease_Success()
        {
            // Arrange
            string repositoryName = "test";
            string version = "v1.0";
            string zipName = "zipName";

            string url = "http://github.com/repos/repo/assets{?name,label}";
            string expectedUploadUrl = url.Substring(0, url.IndexOf("{")) + "?name=zipName&label=zipName";

            CreateReleaseResponseDTO responseDTO = new()
            {
                UploadUrl = url
            };

            httpClient.Setup(mock => mock.Send<CreateReleaseResponseDTO>(It.IsAny<HttpRequestMessage>())).ReturnsAsync(responseDTO);

            // Act
            string actualUploadUrl = await gitHubClient.CreateRelease(repositoryName, version, false, false, zipName);

            // Assert
            Assert.Equal(expectedUploadUrl, actualUploadUrl);
        }
    }
}
