using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.utils
{
    public class JsonUtils
    {
        private static readonly JsonSerializerSettings settings;

        static JsonUtils()
        {
            settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }

        public static T? Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
