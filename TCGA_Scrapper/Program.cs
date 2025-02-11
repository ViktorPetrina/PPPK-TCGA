using DataLayer.Repository;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using TCGA_Scrapper.Utilities;

namespace TCGA_Scrapper
{
    internal class Program
    {
        private const string REQUIRED_TEXT = "IlluminaHiSeq pancan normalized";
        private const string DOWNLOAD_URL_SUBSTRING = "sampleMap%";
        private const string DOWNLOAD_URLS_PATH = "download_urls.txt";
        private const string COMPRESSED_FILES_PATH = @"downloads\";
        private const string DECOMPRESSED_FILES_PATH = @"unpacked\";
        private const string FIRST_LEVEL_URL = "https://xenabrowser.net/datapages/?hub=https://tcga.xenahubs.net:443";

        private const string NO_DOWNLOAD_URL_ERROR = "No download url, exception: ";
        private const string NO_NORMALIZED_ERROR = "No pancan normalized, exception: ";

        private static IList<string> firstLevelUrls = new List<string>();
        private static IList<string> secondLevelUrls = new List<string>();
        private static IList<string> downloadUrls = new List<string>();

        static async Task Main(string[] args)
        {
            ScrapeWeb();

            await DownloadAndUnpackFiles();

            await UploadToDatabse();
        }

        private static async Task UploadToDatabse()
        {
            ISimpleFileRepository repo = new MinioRepository();

            var filePaths = FileUtils.GetAllFilesFromDirectory(DECOMPRESSED_FILES_PATH);
            var contentType = "text/tab-separated-values";

            foreach (var filePath in filePaths)
            {
                var objectName = Path.GetFileName(filePath);
                try
                {
                    await repo.Create(objectName, filePath, contentType);
                    Console.WriteLine($"Successfully uploaded {objectName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to upload {objectName}: {ex.Message}");
                }
            }
        }

        private static async Task DownloadAndUnpackFiles()
        {
            downloadUrls = File.ReadLines(DOWNLOAD_URLS_PATH).ToList();
            await FileUtils.DownloadFiles(COMPRESSED_FILES_PATH, downloadUrls);
            FileUtils.DecompressFilesToDirectory(DECOMPRESSED_FILES_PATH, Directory.GetFiles(COMPRESSED_FILES_PATH, "*.gz"));
        }

        private static void ScrapeWeb()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMicroseconds(0));

                var by = By.CssSelector("a");
                var waitBy = By.CssSelector(".Datapages-module__list___2yM9o li");

                firstLevelUrls = ScrapperUtils.GetUrlsFromUrl(driver, wait, by, waitBy, FIRST_LEVEL_URL);
                Console.WriteLine("First level urls added.\n");

                by = By.LinkText(REQUIRED_TEXT);
                waitBy = By.CssSelector(".Datapages-module__groupList___1h_hi");

                secondLevelUrls = ScrapperUtils.GetUrlsFromUrls(driver, wait, by, waitBy, firstLevelUrls, NO_NORMALIZED_ERROR);
                Console.WriteLine("Second level urls added.\n");

                by = By.XPath($"//a[contains(text(), '{DOWNLOAD_URL_SUBSTRING}')]");
                waitBy = By.CssSelector(".Datapages-module__value___3k05o");

                downloadUrls = ScrapperUtils.GetUrlsFromUrls(driver, wait, by, waitBy, secondLevelUrls, NO_DOWNLOAD_URL_ERROR);
                Console.WriteLine("Download urls added.\n");
            }

            Console.WriteLine("Saving to file...");
            File.WriteAllLines(DOWNLOAD_URLS_PATH, downloadUrls);
        }
    }
}
