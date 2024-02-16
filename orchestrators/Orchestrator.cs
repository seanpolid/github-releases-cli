using GitHubReleasesCLI.clients;
using GitHubReleasesCLI.models;
using GitHubReleasesCLI.utils;
using Microsoft.Extensions.Configuration;

namespace GitHubReleasesCLI.orchestrators
{
    public class Orchestrator
    {
        private const string DEFAULT_ZIP_NAME = "assets";
        private const string DEFAULT_BRANCH = "main";

        public static async Task Run(string[] args, GitHubClient gitHubClient)
        {
            ParsedArgs? parsedArgs = ParseArgs(args);
            if (parsedArgs == null) return;
            Console.WriteLine(parsedArgs);
            Console.WriteLine("Creating release");
            string uploadUri = await gitHubClient.CreateRelease(parsedArgs.RepositoryName, parsedArgs.Version, false, true, parsedArgs.ZipName, parsedArgs.Branch);

            Console.WriteLine("Zipping assets");
            byte[] zippedAssets = FileUtils.Zip(parsedArgs.AssetsPath, parsedArgs.ZipName);

            Console.WriteLine("Uploading assets");
            await gitHubClient.UploadReleaseAsset(uploadUri, zippedAssets);

            Console.WriteLine("Assets were successfully uploaded!");
        }

        public static ParsedArgs? ParseArgs(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder().AddCommandLine(args).Build();

            if (args.Length == 0 || args.Where(arg => arg == "--h").Count() > 0)
            {
                Console.WriteLine("Usage: release <ARGS>");

                Console.WriteLine("\nRequired Args:");
                Console.WriteLine($"{"--v", 5}  Version of release");
                Console.WriteLine($"{"--p", 5}  Path to asset(s)");
                Console.WriteLine($"{"--r",5}  Repository name");

                Console.WriteLine("\nOptional Args:");
                Console.WriteLine($"{"--n",5}  Name of zip file");
                Console.WriteLine($"{"--b",5}  Name of branch (defaults to 'main')");

                return null;
            }

            if (config["v"] is null)
            {
                Console.WriteLine("Please provide a version for the release using the --v flag.");
                return null;
            }
            if (config["r"] is null)
            {
                Console.WriteLine("Please provide the name of the repository using the --r flag.");
                return null;
            }
            if (config["p"] is null)
            {
                Console.WriteLine("Please provide the path to the asset(s) using the --p flag");
                return null;
            }

            var parsedArgs = new ParsedArgs()
            {
                AssetsPath = config["p"],
                RepositoryName = config["r"],
                ZipName = config["n"] ?? DEFAULT_ZIP_NAME,
                Version = config["v"],
                Branch = config["b"] ?? DEFAULT_BRANCH
            };
            parsedArgs.FormatZipName();

            return parsedArgs;
        }
    }
}
