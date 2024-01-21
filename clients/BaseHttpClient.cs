namespace GitHubReleasesCLI.clients
{
    public class BaseHttpClient
    {
        private readonly HttpClient httpClient = new();

        public virtual HttpResponseMessage Send(HttpRequestMessage httpRequestMessage)
        {
            return httpClient.Send(httpRequestMessage);
        }
    }
}
