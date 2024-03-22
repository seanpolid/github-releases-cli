using GitHubReleasesCLI.clients;
using GitHubReleasesCLI.exceptions;
using GitHubReleasesCLI.orchestrators;

namespace GitHubReleasesCLI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string? gitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            if (gitHubToken == null)
            {
                Console.WriteLine("Sorry, could not identify the GitHub token. " +
                    "Please ensure the following environment variable has been set: GITHUB_TOKEN");
                return;
            }

            string? repoOwner = Environment.GetEnvironmentVariable("REPO_OWNER");
            if (repoOwner == null)
            {
                Console.WriteLine("Sorry, could not identify the owner of the repository. " +
                    "Please ensure the following environment variable has been set: REPO_OWNER");
                return;
            }

            try
            {
                var gitHubClient = new GitHubClient(gitHubToken, repoOwner);
                await Orchestrator.Run(args, gitHubClient);
                return;
            }
            catch (Exception ex)
            {
                if (ex is CreateReleaseException || ex is UploadReleaseAssetException || ex is InvalidArgumentException)
                {
                    Console.WriteLine(ex.Message);
                }
                else
                {
                    Console.WriteLine("An unkown exception occurred: {0}", ex.Message);
                }
            }
        }
    }
}