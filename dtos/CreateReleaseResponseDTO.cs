using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.dtos
{
    public class CreateReleaseResponseDTO
    {
        public string? Url { get; set; }

        public string? UploadUrl { get; set; }

        public string? AssetUrl { get; set; }
    }
}
