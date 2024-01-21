using GitHubReleasesCLI.dtos;
using GitHubReleasesCLI.enums;
using GitHubReleasesCLI.exceptions;
using GitHubReleasesCLI.NewFolder;
using GitHubReleasesCLI.utils;
using System.Net;
using System.Text;

namespace GitHubReleasesCLI.clients
{
    public class GitHubClient
    {
        private readonly string GITHUB_TOKEN;
        private readonly string REPO_OWNER;
        private readonly BaseHttpClient httpClient;

        public GitHubClient(string gitHubToken, string repoOwner)
        {
            GITHUB_TOKEN = gitHubToken;
            REPO_OWNER = repoOwner;
            httpClient = new BaseHttpClient();
        }

        public GitHubClient(string githubToken, string repoOwner, BaseHttpClient client)
        {
            GITHUB_TOKEN = githubToken;
            REPO_OWNER = repoOwner;
            this.httpClient = client;
        }

        /// <summary>
        /// Creates a release for the specified repository and returns a uri to upload assets.
        /// </summary>
        /// <param name="repositoryname"></param>
        /// <param name="zipname"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public virtual async Task<string> CreateRelease(string repositoryName, string version, bool makeDraft, bool makeLatest, string zipName)
        {
            string uri = $"https://api.github.com/repos/{REPO_OWNER}/{repositoryName}/releases";
            CreateReleaseRequestDTO createReleaseRequestDTO = new()
            {
                TagName = version,
                Name = version,
                Draft = makeDraft,
                Body = "",
                MakeLatest = $"{makeLatest}",
                TargetCommitish = "main"
            };

            HttpRequestMessage request = CreateRequestMessage(HttpMethod.Post, uri, AcceptType.GITHUB_JSON, ContentType.JSON, createReleaseRequestDTO);
            HttpResponseMessage response = httpClient.Send(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                throw new CreateReleaseException("Could not create the release because it already exists. Please make the new release unique.");
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new CreateReleaseException("A problem occurred while creating the release: " + responseBody);
            }

            var responseDTO = JsonUtils.Deserialize<CreateReleaseResponseDTO>(responseBody);
            return CreateUploadUrl(responseDTO.UploadUrl, zipName);
        }

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri, AcceptType acceptType, ContentType contentType, object body)
        {
            return new()
            {
                Method = method,
                RequestUri = new Uri(uri),
                Headers =
                {
                    {"Accept", GetAcceptTypeString(acceptType)},
                    {"Authorization", $"Bearer {GITHUB_TOKEN}"},
                    {"X-GitHub-Api-Version", "2022-11-28"},
                    {"User-Agent", REPO_OWNER},
                },
                Content = GetContent(body, contentType)
            };
        }

        private HttpContent GetContent(object body, ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.JSON:
                    return new StringContent(JsonUtils.Serialize(body), Encoding.UTF8, "application/json");
                case ContentType.ZIP:
                    var content = new ByteArrayContent((byte[])body);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
                    return content;
                default:
                    throw new ArgumentException(contentType + " is not defined.");
            }
        }

        private static string GetAcceptTypeString(AcceptType acceptType)
        {
            switch (acceptType)
            {
                case AcceptType.GITHUB_JSON:
                    return "application/vnd.github+json";
                case AcceptType.OCTET_STREAM:
                    return "application/octet-stream";
                default:
                    return "application/json";
            }
        }

        private static string CreateUploadUrl(string? uploadUrl, string zipName)
        {
            string baseUrl = uploadUrl.Substring(0, uploadUrl.IndexOf('{'));
            return $"{baseUrl}?name={zipName}.zip&label={zipName}.zip";
        }

        public async Task UploadReleaseAsset(string uri, byte[] zippedAssets)
        {
            HttpRequestMessage request = CreateRequestMessage(HttpMethod.Post, uri, AcceptType.GITHUB_JSON, ContentType.ZIP, zippedAssets);
            HttpResponseMessage response = httpClient.Send(request);
            
            string message = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new UploadReleaseAssetException("A problem occurred while uploading the asset(s): " + message);
            } 
        }
    }
}
