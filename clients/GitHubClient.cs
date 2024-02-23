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
        public virtual async Task<string> CreateRelease(string repositoryName, string version, bool makeDraft, bool makeLatest, string zipName, string branch)
        {
            string uri = $"https://api.github.com/repos/{REPO_OWNER}/{repositoryName}/releases";
            CreateReleaseRequestDTO createReleaseRequestDTO = new()
            {
                TagName = version,
                Name = version,
                Draft = makeDraft,
                Body = "",
                MakeLatest = $"{makeLatest}",
                TargetCommitish = branch
            };

            HttpRequestMessage request = CreateRequestMessage(HttpMethod.Post, uri, AcceptType.GITHUB_JSON, ContentType.JSON, createReleaseRequestDTO);
            HttpResponseMessage response = httpClient.Send(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                string message = GetValidationErrorMessage(responseBody);
                throw new CreateReleaseException(message);
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new CreateReleaseException("A problem occurred while creating the release: " + responseBody);
            }

            var responseDTO = JsonUtils.Deserialize<CreateReleaseResponseDTO>(responseBody);
            return responseDTO.UploadUrl;
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

        private static HttpContent GetContent(object body, ContentType contentType)
        {
            string bodyString = JsonUtils.Serialize(body);

            switch (contentType)
            {
                case ContentType.JSON:
                    return new StringContent(bodyString, Encoding.UTF8, "application/json");
                case ContentType.ZIP:
                case ContentType.OCTET_STREAM:
                    string header = contentType == ContentType.ZIP ? "application/zip" : "application/octet-stream";

                    var content = new ByteArrayContent((byte[])body);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(header);
                    return content;
                default:
                    throw new ArgumentException(contentType + " is not defined.");
            }
        }

        private static string GetValidationErrorMessage(string responseBody)
        {
            StringBuilder builder = new("Could not create the release due to validation errors.");

            var responseDTO = JsonUtils.Deserialize<CreateReleaseValidationErrorsDTO>(responseBody);
            if (responseDTO == null || responseDTO.Errors.Count == 0)
            {
                return builder.ToString();
            }

            builder.Append(" Please refer to the following:");
            foreach (ValidationErrorDTO error in responseDTO.Errors)
            {
                if (error.Code is null) continue;

                builder.Append("\n- ");
                if (error.Code == "custom")
                {
                    builder.Append(error.Message);
                }
                else if (error.Code == "invalid")
                {
                    builder.Append(error.Field);
                    builder.Append(" is invalid.");
                }
                else if (error.Code == "already_exists")
                {
                    builder.Append(error.Field);
                    builder.Append(" already exists.");
                }
            }

            return builder.ToString();
        }

        public async Task UploadReleaseAsset(string uri, byte[] data, ContentType type)
        {
            HttpRequestMessage request = CreateRequestMessage(HttpMethod.Post, uri, AcceptType.GITHUB_JSON, type, data);
            HttpResponseMessage response = httpClient.Send(request);
            
            string message = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new UploadReleaseAssetException("A problem occurred while uploading the asset(s): " + message);
            } 
        }

        public static string CreateUploadUrl(string? uploadUrl, string fileName)
        {
            string baseUrl = uploadUrl.Substring(0, uploadUrl.IndexOf('{'));
            return $"{baseUrl}?name={fileName}&label={fileName}";
        }
    }
}
