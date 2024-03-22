using GitHubReleasesCLI.clients;
using GitHubReleasesCLI.enums;
using GitHubReleasesCLI.exceptions;
using GitHubReleasesCLI.models;
using GitHubReleasesCLI.utils;
using Microsoft.Extensions.Configuration;

namespace GitHubReleasesCLI.orchestrators
{
    public class Orchestrator
    {
        private const string DEFAULT_ZIP_NAME = "assets";
        private const string DEFAULT_BRANCH = "main";
        private static readonly string BASE_DIRECTORY = Directory.GetCurrentDirectory();

        public static async Task Run(string[] args, GitHubClient gitHubClient)
        {
            ParsedArgs? parsedArgs = ParseArgs(args);
            if (parsedArgs == null) return;

            Console.WriteLine("Creating release");
            string uploadUriTemplate = await gitHubClient.CreateRelease(parsedArgs.RepositoryName, parsedArgs.Version, false, true, parsedArgs.ZipName, parsedArgs.Branch);

            byte[] assets = ReadAssets(parsedArgs);

            if (parsedArgs.KeyPath != null)
            {
                Console.WriteLine("Generating Digital signature");
                byte[] signature = SignatureUtils.Sign(BASE_DIRECTORY, assets, parsedArgs.KeyPath);

                Console.WriteLine("Uploading signature");
                string signatureUploadUri = GitHubClient.CreateUploadUrl(uploadUriTemplate, "signature.dat");
                await gitHubClient.UploadReleaseAsset(signatureUploadUri, signature, ContentType.OCTET_STREAM);
            }

            Console.WriteLine("Uploading assets");
            string assetUploadUri = GitHubClient.CreateUploadUrl(uploadUriTemplate, $"{parsedArgs.ZipName}.zip");
            await gitHubClient.UploadReleaseAsset(assetUploadUri, assets, ContentType.ZIP);

            Console.WriteLine("Assets were successfully uploaded!");
        }

        private static byte[] ReadAssets(ParsedArgs parsedArgs)
        {
            byte[] assets;
            if (parsedArgs.ZipAssets)
            {
                Console.WriteLine("Zipping assets");
                assets = FileUtils.Zip(BASE_DIRECTORY, parsedArgs.AssetsPath, parsedArgs.ZipName, parsedArgs.IncludeBaseDirectory);
            }
            else if (!Directory.Exists(parsedArgs.AssetsPath))
            {
                assets = FileUtils.ReadFile(BASE_DIRECTORY, parsedArgs.AssetsPath);
            }
            else
            {
                throw new InvalidArgumentException("Could not read in assets because folder was provided. Please provide the --z flag if your asset path is a folder.");
            }

            return assets;
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
                Console.WriteLine($"{"--r", 5}  Repository name");

                Console.WriteLine("\nOptional Args:");
                Console.WriteLine($"{"--n",5}  Name of zip file");
                Console.WriteLine($"{"--b",5}  Name of branch (defaults to 'main')");
                Console.WriteLine($"{"--k",5}  Path to private key file (PEM form)");
                Console.WriteLine($"{"--i",5}  Include base directory");
                Console.WriteLine($"{"--z",5}  Zip assets (defaults to no zip)");

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

            bool includeBaseDirectory = args.Where(args => args.Equals("--i")).Count() > 0;
            bool zipAssets = args.Where(args => args.Equals("--z")).Count() > 0;

            var parsedArgs = new ParsedArgs()
            {
                AssetsPath = config["p"],
                RepositoryName = config["r"],
                ZipName = config["n"] ?? DEFAULT_ZIP_NAME,
                Version = config["v"],
                Branch = config["b"] ?? DEFAULT_BRANCH,
                KeyPath = config["k"],
                IncludeBaseDirectory = includeBaseDirectory,
                ZipAssets = zipAssets
            };
            parsedArgs.FormatZipName();

            return parsedArgs;
        }
    }
}
