using GitHubReleasesCLI.dtos;
using GitHubReleasesCLI.enums;
using GitHubReleasesCLI.NewFolder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace GitHubReleasesCLI.clients
{
    internal class GitHubClient
    {
        private readonly string GITHUB_TOKEN;
        private readonly string REPO_OWNER;
        private readonly HttpClient client;
        private readonly JsonSerializerSettings settings;

        public GitHubClient(string gitHubToken, string repoOwner)
        {
            GITHUB_TOKEN = gitHubToken;
            REPO_OWNER = repoOwner;
            client = new HttpClient();

            settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public GitHubClient(string githubToken, string repoOwner, HttpClient client)
        {
            GITHUB_TOKEN = githubToken;
            REPO_OWNER = repoOwner;
            this.client = client;

            settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
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
            var uri = $"https://api.github.com/repos/{REPO_OWNER}/{repositoryName}/releases";
            var createReleaseRequestDTO = new CreateReleaseRequestDTO()
            {
                TagName = version,
                Name = version,
                Draft = makeDraft,
                Body = "",
                MakeLatest = $"{makeLatest}",
                TargetCommitish = "main"
            };

            var request = CreateRequestMessage(HttpMethod.Post, uri, AcceptType.GITHUB_JSON, ContentType.JSON, createReleaseRequestDTO);
            var response = await Send<CreateReleaseResponseDTO>(request);

            return CreateUploadUrl(response.UploadUrl, zipName);
        }

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri, AcceptType acceptType, ContentType contentType, object body)
        {
            string serializedBody = JsonConvert.SerializeObject(body, Formatting.Indented, settings);
            return new()
            {
                Method = method,
                RequestUri = new Uri(uri),
                Headers =
                {
                    {"Accept", GetAcceptTypeString(acceptType)},
                    {"Authorization", $"Bearer {GITHUB_TOKEN}"},
                    {"X-GitHub-Api-Version", "2022-11-28"},
                    {"User-Agent", REPO_OWNER}
                },
                Content = new StringContent(serializedBody, Encoding.UTF8, "application/json")
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

        private async Task<T> Send<T>(HttpRequestMessage httpRequestMessage)
        {
            var response= client.Send(httpRequestMessage);
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body, settings);
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
