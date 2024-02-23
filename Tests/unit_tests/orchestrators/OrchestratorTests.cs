using GitHubReleasesCLI.orchestrators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHubReleasesCLI.models;
using GitHubReleasesCLI.orchestrators;
using GitHubReleasesCLI.clients;
using Xunit.Sdk;

namespace Tests.unit_tests.orchestrators
{
    public class OrchestratorTests
    {
        [Fact]
        public void ParseArgs_NoArgs_Null()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act
            ParsedArgs? parsedArgs = Orchestrator.ParseArgs(args);

            // Assert
            Assert.Null(parsedArgs);
        }

        [Fact]
        public void ParseArgs_Help_Null()
        {
            // Arrange
            string[] args = { "--h" };

            // Act
            ParsedArgs? parsedArgs = Orchestrator.ParseArgs(args);

            // Assert
            Assert.Null(parsedArgs);
        }

        [Fact]
        public void ParseArgs_NoVersion_Null()
        {
            // Arrange
            string[] args = { "--r", "repo name", "--p", "path" };

            // Act
            ParsedArgs? parsedArgs = Orchestrator.ParseArgs(args);

            // Assert
            Assert.Null(parsedArgs);
        }

        [Fact]
        public void ParseArgs_NoRepositoryName_Null()
        {
            // Arrange
            string[] args = { "--v", "version", "--p", "path" };

            // Act
            ParsedArgs? parsedArgs = Orchestrator.ParseArgs(args);

            // Assert
            Assert.Null(parsedArgs);
        }

        [Fact]
        public void ParseArgs_NoAssetPath_Null()
        {
            // Arrange
            string[] args = { "--v", "version", "--r", "repository name" };

            // Act
            ParsedArgs? parsedArgs = Orchestrator.ParseArgs(args);

            // Assert
            Assert.Null(parsedArgs);
        }

        [Fact]
        public void ParseArgs_AllArgsButZipNameProvided_Success()
        {
            // Arrange
            string[] args = { "--v", "version", "--r", "repository name", "--p", "path"};
            ParsedArgs expected = new()
            {
                Version = "version",
                RepositoryName = "repository name",
                AssetsPath = "path",
                ZipName = "assets",
                Branch = "main"
            };

            // Act
            ParsedArgs? actual = Orchestrator.ParseArgs(args);

            // Assert
            Assert.StrictEqual(expected, actual);
        }

        [Fact]
        public void ParseArgs_AllArgsProvided_Success()
        {
            // Arrange
            string[] args = { "--v", "version", "--r", "repository name", "--p", "path", "--n", "zip name", "--k", "key path" };
            ParsedArgs expected = new()
            {
                Version = "version",
                RepositoryName = "repository name",
                AssetsPath = "path",
                ZipName = "zip name",
                Branch = "main",
                KeyPath = "key path"
            };

            // Act
            ParsedArgs? actual = Orchestrator.ParseArgs(args);

            // Assert
            Assert.StrictEqual(expected, actual);
        }
    }
}
