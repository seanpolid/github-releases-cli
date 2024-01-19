using GitHubReleasesCLI.dtos;
using GitHubReleasesCLI.enums;
using GitHubReleasesCLI.NewFolder;
using GitHubReleasesCLI.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        public async Task<string> CreateRelease(string repositoryName, string version, bool makeDraft, bool makeLatest, string zipName)
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
            CreateReleaseResponseDTO response = await httpClient.Send<CreateReleaseResponseDTO>(request);

            return CreateUploadUrl(response.UploadUrl, zipName);
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
                Content = new StringContent(JsonUtils.Serialize(body), Encoding.UTF8, "application/json")
            };
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

        private static string GetContentTypeString(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.ZIP:
                    return "application/zip";
                case ContentType.JSON:
                    return "application/json";
                default:
                    return "";
            }
        }

        private static string CreateUploadUrl(string? uploadUrl, string zipName)
        {
            string baseUrl = uploadUrl.Substring(0, uploadUrl.IndexOf('{'));
            return $"{baseUrl}?name={zipName}&label={zipName}";
        }

        public void UploadReleaseAsset(string uri, string assetPath)
        {

        }
    }
}
