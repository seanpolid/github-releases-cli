using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.utils
{
    public class FileUtils
    {
        /// <summary>
        /// Zips the contents found at the specified path. If it is a single file, it will
        /// create a new file called 'asset.zip'. Otherwise, it will use the name of the folder 
        /// for the zip file's name and it will include the base directory in the zip.
        /// </summary>
        /// <param name="path"></param>
        public static void Zip(string path)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(baseDirectory, path);
            }

            if (File.Exists(path))
            {
                string zipPath = Path.Combine(baseDirectory, "asset.zip");
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                using ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
                zip.CreateEntryFromFile(path, Path.GetFileName(path));
            }
            else if (Directory.Exists(path))
            {
                string zipFile = $"{path}.zip";
                string zipPath = Path.Combine(baseDirectory, zipFile);
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                ZipFile.CreateFromDirectory(path, zipPath, CompressionLevel.Optimal, true);
            }
            else
            {
                throw new FileNotFoundException($"{path} does not exist");
            }
        }
    }
}