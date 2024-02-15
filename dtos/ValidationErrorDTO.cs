using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.dtos
{
    internal class ValidationErrorDTO
    {
        public string? Resource { get; set; }

        public string? Code { get; set; }

        public string? Field { get; set; }

        public string? Message { get; set; }
    }
}
