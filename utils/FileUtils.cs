using System.IO.Compression;

namespace GitHubReleasesCLI.utils
{
    public class FileUtils
    {
        /// <summary>
        /// Zips the contents found at the specified path. Outputs a file called 'assets.zip'.
        /// Returns the zipped assets as a byte array.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static byte[] Zip(string path, string zipName)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(baseDirectory, path);
            }

            if (File.Exists(path))
            {
                string zipPath = Path.Combine(baseDirectory, $"{zipName}.zip");
                ZipFile(path, zipPath);

                return File.ReadAllBytes(zipPath); ;
            }
            else if (Directory.Exists(path))
            {
                string zipFile = $"{zipName}.zip";
                string zipPath = Path.Combine(baseDirectory, zipFile);
                ZipFolder(path, zipPath);

                return File.ReadAllBytes(zipPath);
            }
            else
            {
                throw new FileNotFoundException($"{path} does not exist");
            }
        }

        private static void ZipFile(string path, string zipPath)
        {
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            using ZipArchive zip = System.IO.Compression.ZipFile.Open(zipPath, ZipArchiveMode.Create);
            zip.CreateEntryFromFile(path, Path.GetFileName(path));
        }

        private static void ZipFolder(string path, string zipPath)
        {
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            System.IO.Compression.ZipFile.CreateFromDirectory(path, zipPath, CompressionLevel.Optimal, true);
        }
    }
}