using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.NewFolder
{
    public class CreateReleaseRequestDTO
    {
        public string? TagName { get; set; }

        public string? Name { get; set; }

        public string? TargetCommitish { get; set; }

        public string? Body { get; set; }

        public bool Draft { get; set; }

        public bool Prerelease { get; set; }

        public bool GenerateReleaseNotes { get; set; }

        public string? MakeLatest { get; set; }
    }
}
