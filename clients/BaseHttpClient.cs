namespace GitHubReleasesCLI.clients
{
    public class BaseHttpClient
    {
        private readonly HttpClient httpClient;

        public BaseHttpClient()
        {
            httpClient = new();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public virtual HttpResponseMessage Send(HttpRequestMessage httpRequestMessage)
        {
            return httpClient.Send(httpRequestMessage);
        }
    }
}
