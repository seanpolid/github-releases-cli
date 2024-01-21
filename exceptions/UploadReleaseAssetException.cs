using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.exceptions
{
    public class UploadReleaseAssetException : Exception
    {
        public UploadReleaseAssetException(string message) : base(message)
        {
        }
    }
}
