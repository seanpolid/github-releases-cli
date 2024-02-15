using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.dtos
{
    internal class CreateReleaseValidationErrorsDTO
    {
        public string? Message { get; set; }

        public List<ValidationErrorDTO> Errors { get; set; } = new List<ValidationErrorDTO>();
    }
}
