using System.IO.Compression;

namespace TCGA_Scrapper.Utilities
{
    public static class FileUtils
    {
        public static IEnumerable<string> GetAllFilesFromDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                return new List<string>(Directory.GetFiles(directory));
            }
            else
            {
                throw new DirectoryNotFoundException($"The directory '{directory}' does not exist.");
            }
        }

        public static void DecompressFile(string source, string destination)
        {
            using (FileStream compressedStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(destination, FileMode.Create, FileAccess.Write))
            using (GZipStream decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(outputStream);
            }
        }

        public static async Task DownloadFiles(string dir, IEnumerable<string> urls)
        {
            Directory.CreateDirectory(dir);

            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) })
            {
                int counter = 1;
                foreach (string url in urls)
                {
                    string fileName = Path.GetFileName(new Uri(url).LocalPath);

                    string uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{counter}{Path.GetExtension(fileName)}";

                    string filePath = Path.Combine(dir, uniqueFileName);

                    try
                    {
                        Console.WriteLine($"\nDownloading {uniqueFileName}...");
                        byte[] fileBytes = await client.GetByteArrayAsync(url);
                        await File.WriteAllBytesAsync(filePath, fileBytes);
                        Console.WriteLine($"{uniqueFileName} downloaded.");
                    }
                    catch (TaskCanceledException e)
                    {
                        Console.WriteLine($"Download timeout. Exception: {e.Message}");
                    }

                    counter++;
                }
            }
        }

        public static void DecompressFilesToDirectory(string path, IEnumerable<string> files)
        {
            Directory.CreateDirectory(path);

            foreach (var file in files)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string destination = Path.Combine(path, fileName);
                    DecompressFile(file, destination);
                    Console.WriteLine($"File {file} extracted successfully to {destination}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred while extracting {file}: {e.Message}");
                }
            }
        }
    }
}
