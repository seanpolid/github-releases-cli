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

        public string Branch { get; set; }

        public string? KeyPath { get; set; }

        public ParsedArgs()
        {
            AssetsPath = string.Empty;
            RepositoryName = string.Empty;
            ZipName = string.Empty;
            Version = string.Empty;
            Branch = string.Empty;
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
                   Version == args.Version &&
                   Branch == args.Branch &&
                   KeyPath == args.KeyPath;
        }

        public override string? ToString()
        {
            return String.Format("{0} {1} {2} {3} {4}", AssetsPath, RepositoryName, ZipName, Version, Branch);
        }
    }
}
