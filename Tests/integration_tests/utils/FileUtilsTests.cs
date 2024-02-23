using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHubReleasesCLI.utils;

namespace Tests.integration_tests.utils
{
    public class FileUtilsTests : IDisposable
    {
        private readonly string zipName = "assets";
        private readonly string zipFile = "assets.zip";
        private readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private string? expectedDirectory;
        private string? testDirectory;
        private string? file1Path;
        private string? file2Path;
        private string? file3Path;

        public FileUtilsTests() 
        {
            CreateTestFilesAndFolders();
        }

        private void CreateTestFilesAndFolders()
        {
            expectedDirectory = Path.Combine(baseDirectory, "expected");
            Directory.CreateDirectory(expectedDirectory);

            testDirectory = Path.Combine(baseDirectory, "test");
            Directory.CreateDirectory(testDirectory);

            file1Path = Path.Combine(baseDirectory, "file1.txt");
            file2Path = Path.Combine(testDirectory, "file2.txt");
            file3Path = Path.Combine(testDirectory, "file3.txt");

            File.Create(file1Path).Close();
            File.Create(file2Path).Close();
            File.Create(file3Path).Close();
        }

        public void Dispose()
        {
            Directory.Delete(expectedDirectory, true);
            Directory.Delete(testDirectory, true);
            File.Delete(file1Path);

            IEnumerable<string> zipFilesToDelete = Directory.EnumerateFiles(baseDirectory)
                                                            .Where(file => file.Contains(".zip"))
                                                            .ToList();
            foreach(string zipFile in zipFilesToDelete)
            {
                File.Delete(zipFile);
            }
        }

        [Fact]
        public void Zip_RelativeFile_Success()
        {
            // Arrange
            string path = "./file1.txt";

            string zipFilePath = Path.Combine(baseDirectory, zipFile);

            // Act
            byte[] zippedFile = FileUtils.Zip(baseDirectory, path, zipName);
            ZipFile.ExtractToDirectory(zipFilePath, expectedDirectory);     // Confirm file can be unzipped properly

            // Assert
            bool fileExists = File.Exists(Path.Combine(expectedDirectory, "file1.txt"));
            Assert.True(fileExists);

            Assert.True(File.Exists(zipFilePath));
            Assert.Equal(File.ReadAllBytes(zipFilePath), zippedFile);
        }

        [Fact]
        public void Zip_AbsoluteFile_Success()
        {
            // Arrange
            string path = Path.Combine(baseDirectory, "file1.txt");

            string zipFilePath = Path.Combine(baseDirectory, zipFile);

            // Act
            byte[] zippedFile = FileUtils.Zip(baseDirectory, path, zipName);
            ZipFile.ExtractToDirectory(zipFilePath, expectedDirectory);     // Confirm file can be unzipped properly

            // Assert
            bool fileExists = File.Exists(Path.Combine(expectedDirectory, "file1.txt"));
            Assert.True(fileExists);
            Assert.True(File.Exists(zipFilePath));
            Assert.Equal(File.ReadAllBytes(zipFilePath), zippedFile);
        }

        [Fact]
        public void Zip_Folder_Success()
        {
            // Assert
            string zipFilePath = Path.Combine(baseDirectory, zipFile);
            
            // Act
            byte[] zippedFile = FileUtils.Zip(baseDirectory, testDirectory, zipName);
            ZipFile.ExtractToDirectory(zipFilePath, expectedDirectory);    // Confirm file can be unzipped properly

            //Assert
            Assert.True(File.Exists(zipFilePath));
            Assert.Equal(File.ReadAllBytes(zipFilePath), zippedFile);
            AssertDirectoriesAreEqual(expectedDirectory, testDirectory);
        }

        private static void AssertDirectoriesAreEqual(string parentFolder, string testDirectory)
        {
            // Make sure base directory is present
            string[] directories = Directory.GetDirectories(parentFolder);
            Assert.Single(directories);
            Assert.Equal(Path.GetFileName(directories[0]), Path.GetFileName(testDirectory));

            // Make sure the nested files are the same
            HashSet<string> actualFiles = Directory.GetFiles(directories[0])
                                                   .Select(path => Path.GetFileName(path))
                                                   .ToHashSet();
            HashSet<string> expectedFiles = Directory.GetFiles(testDirectory)
                                                     .Select(path => Path.GetFileName(path))
                                                     .ToHashSet();
            Assert.True(expectedFiles.SetEquals(actualFiles));
        }

        [Fact]
        public void Zip_NonExistantPath_Exception()
        {
            // Arrange
            string path = "nonexistant-file.txt";

            // Act and Assert
            Assert.Throws<FileNotFoundException>(() =>
            {
                FileUtils.Zip(baseDirectory, path, zipName);
            });
        }
    }
}
