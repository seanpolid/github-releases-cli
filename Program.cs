using GitHubReleasesCLI.clients;

namespace GitHubReleasesCLI
{
    internal class Program
    {
        private static readonly string GITHUB_TOKEN = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        private static readonly string REPO_OWNER = Environment.GetEnvironmentVariable("REPO_OWNER");

        static async Task Main(string[] args)
        {
            var variables = Environment.GetEnvironmentVariables();
            if (GITHUB_TOKEN == null) 
            {
                Console.WriteLine("Sorry, could not identify the GitHub token. " +
                    "Please ensure the following environment variable has been set: GITHUB_TOKEN");
                return;
            }
            if (REPO_OWNER == null)
            {
                Console.WriteLine("Sorry, could not identify the owner of the repository. " +
                    "Please ensure the following environment variable has been set: REPO_OWNER");
                return;
            }

            var gitHubClient = new GitHubClient(GITHUB_TOKEN, REPO_OWNER);

            var repositoryName = "test-repo";
            var zipName = "zipName";
            var version = "v1.0";
            var uploadUri = await gitHubClient.CreateRelease(repositoryName, version, true, true, zipName);

            Console.WriteLine(uploadUri);
        }
    }
}