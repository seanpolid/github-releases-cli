using System.Text;
using System.Net;
using GitHubReleasesCLI.clients;
using GitHubReleasesCLI.dtos;
using GitHubReleasesCLI.utils;
using GitHubReleasesCLI.exceptions;

namespace Tests.unit_tests.clients
{
    public class GitHubClientTests
    {
        private GitHubClient gitHubClient;
        private Mock<BaseHttpClient> mockHttpClient;

        public GitHubClientTests()
        {
            string gitHubToken = "token";
            string repoOwner = "owner";
            mockHttpClient = new Mock<BaseHttpClient>();

            gitHubClient = new GitHubClient(gitHubToken, repoOwner, mockHttpClient.Object);
        }

        [Fact]
        public async Task CreateRelease_201_Success()
        {
            // Arrange
            string repositoryName = "test";
            string version = "v1.0";
            string zipName = "zipName";

            string url = "http://github.com/repos/repo/assets{?name,label}";
            string expectedUploadUrl = url.Substring(0, url.IndexOf("{")) + $"?name={zipName}.zip&label={zipName}.zip";

            CreateReleaseResponseDTO responseDTO = new()
            {
                UploadUrl = url
            };

            HttpResponseMessage httpResponseMessage = new();
            httpResponseMessage.Content = new StringContent(JsonUtils.Serialize(responseDTO), Encoding.UTF8, "application/json");
            httpResponseMessage.StatusCode = HttpStatusCode.Created;

            mockHttpClient.Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>())).Returns(httpResponseMessage);

            // Act
            string actualUploadUrl = await gitHubClient.CreateRelease(repositoryName, version, false, false, zipName);

            // Assert
            Assert.Equal(expectedUploadUrl, actualUploadUrl);
        }

        [Fact]
        public void CreateRelease_422_Exception()
        {
            // Arrange
            string repositoryName = "test";
            string version = "v1.0";
            string zipName = "zipName";

            string url = "http://github.com/repos/repo/assets{?name,label}";
            string expectedUploadUrl = url.Substring(0, url.IndexOf("{")) + $"?name={zipName}.zip&label={zipName}.zip";

            HttpResponseMessage httpResponseMessage = new();
            httpResponseMessage.Content = new StringContent("Message", Encoding.UTF8, "application/json");
            httpResponseMessage.StatusCode = HttpStatusCode.UnprocessableEntity;

            mockHttpClient.Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>())).Returns(httpResponseMessage);

            // Act and Assert
            Assert.ThrowsAsync<CreateReleaseException>(async () =>
            {
                await gitHubClient.CreateRelease(repositoryName, version, false, false, zipName);
            });
        }

        [Fact]
        public void CreateRelease_503_Exception()
        {
            // Arrange
            string repositoryName = "test";
            string version = "v1.0";
            string zipName = "zipName";

            string url = "http://github.com/repos/repo/assets{?name,label}";
            string expectedUploadUrl = url.Substring(0, url.IndexOf("{")) + $"?name={zipName}.zip&label={zipName}.zip";

            HttpResponseMessage httpResponseMessage = new();
            httpResponseMessage.Content = new StringContent("Message", Encoding.UTF8, "application/json");
            httpResponseMessage.StatusCode = HttpStatusCode.ServiceUnavailable;

            mockHttpClient.Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>())).Returns(httpResponseMessage);

            // Act and Assert
            Assert.ThrowsAsync<CreateReleaseException>(async () =>
            {
                await gitHubClient.CreateRelease(repositoryName, version, false, false, zipName);
            });
        }
    }
}
