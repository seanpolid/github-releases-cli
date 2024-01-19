using GitHubReleasesCLI.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.clients
{
    public class BaseHttpClient
    {
        private readonly HttpClient httpClient = new();

        public async virtual Task<T> Send<T>(HttpRequestMessage httpRequestMessage)
        {
            HttpResponseMessage response = httpClient.Send(httpRequestMessage);
            string body = await response.Content.ReadAsStringAsync();
            return JsonUtils.Deserialize<T>(body);
        }
    }
}
