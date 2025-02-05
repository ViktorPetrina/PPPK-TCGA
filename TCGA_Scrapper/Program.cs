using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using TCGA_Scrapper.Utilities;

namespace TCGA_Scrapper
{
    // TODO: refaktorirat
    internal class Program
    {
        private const long WAIT_LENGTH = 10;

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
        private static IList<string> compressedFiles = new List<string>();

        static async Task Main(string[] args)
        {
            ScrapeWeb();
            LoadUrlsFromFile();
            await DownloadFiles();
            LoadCompressedFiles();
            UnpackFiles();
        }

        private static void LoadCompressedFiles()
        {
            Directory.GetFiles(COMPRESSED_FILES_PATH, "*.gz").ToList().ForEach(compressedFiles.Add);
        }

        private static void UnpackFiles()
        {
            Directory.CreateDirectory(DECOMPRESSED_FILES_PATH);

            foreach (var file in compressedFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string destination = Path.Combine(DECOMPRESSED_FILES_PATH, fileName);
                    FileUtils.DecompressFile(file, destination);
                    Console.WriteLine($"File {file} extracted successfully to {destination}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred while extracting {file}: {e.Message}");
                }
            }
        }

        private static void LoadUrlsFromFile()
        {
            File.ReadLines(DOWNLOAD_URLS_PATH).ToList().ForEach(downloadUrls.Add);
        }

        private static async Task DownloadFiles()
        {
            Directory.CreateDirectory(COMPRESSED_FILES_PATH);

            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromMinutes(1) })
            {
                int counter = 1;
                foreach (string url in downloadUrls)
                {
                    string fileName = Path.GetFileName(new Uri(url).LocalPath);

                    string uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{counter}{Path.GetExtension(fileName)}";

                    string filePath = Path.Combine(COMPRESSED_FILES_PATH, uniqueFileName);

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

        private static void FillFirstLevelUrls(IWebDriver driver, WebDriverWait wait) 
        {
            driver.Navigate().GoToUrl(FIRST_LEVEL_URL);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_LENGTH));
            wait.Until(d => d.FindElements(By.CssSelector(".Datapages-module__list___2yM9o li")).Count >= 6);

            var listItems = driver.FindElements(By.CssSelector(".Datapages-module__list___2yM9o li"));
            foreach (var item in listItems)
            {
                var aTag = item.FindElement(By.CssSelector("a"));
                string href = aTag.GetAttribute("href");
                firstLevelUrls.Add(href);
                Console.WriteLine($"Added url: {href}\n");
            }
        }

        private static void FillSecondLevelUrls(IWebDriver driver, WebDriverWait wait)
        {
            foreach (var url in firstLevelUrls)
            {
                driver.Navigate().GoToUrl(url);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_LENGTH));
                wait.Until(d => d.FindElement(By.CssSelector(".Datapages-module__groupList___1h_hi")));

                try
                {
                    var aTag = driver.FindElement(By.LinkText(REQUIRED_TEXT));
                    string href = aTag.GetAttribute("href");
                    secondLevelUrls.Add(href);
                    Console.WriteLine($"Added url: {href}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No pancan normalized, exception: " + ex.Message + '\n');
                }
            }
        }

        private static void FillThirdLevelUrls(IWebDriver driver, WebDriverWait wait)
        {
            foreach (var url in secondLevelUrls)
            {
                driver.Navigate().GoToUrl(url);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_LENGTH));
                wait.Until(d => d.FindElement(By.CssSelector(".Datapages-module__value___3k05o")));

                try
                {
                    var aTag = driver.FindElement(By.XPath($"//a[contains(text(), '{DOWNLOAD_URL_SUBSTRING}')]"));
                    string href = aTag.GetAttribute("href");
                    downloadUrls.Add(href);
                    Console.WriteLine($"Added url: {href}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No download url, exception: " + ex.Message + '\n');
                }
            }
        }
    }
}
