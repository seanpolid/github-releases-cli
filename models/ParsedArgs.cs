using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.models
{
    public class ParsedArgs
    {
        public string AssetsPath { get; set; }

        public string RepositoryName { get; set; }

        public string ZipName { get; set; }

        public string Version { get; set; }

        public ParsedArgs()
        {
            AssetsPath = string.Empty;
            RepositoryName = string.Empty;
            ZipName = string.Empty;
            Version = string.Empty;
        }

        public void FormatZipName()
        {
            if (ZipName != null && ZipName.Contains(".zip"))
            {
                ZipName = ZipName.Substring(0, ZipName.IndexOf(".zip"));
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is ParsedArgs args &&
                   AssetsPath == args.AssetsPath &&
                   RepositoryName == args.RepositoryName &&
                   ZipName == args.ZipName &&
                   Version == args.Version;
        }
    }
}
