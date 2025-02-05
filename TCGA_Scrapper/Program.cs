using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.Metrics;

namespace TCGA_Scrapper
{
    // TODO: refaktorirat
    internal class Program
    {
        private const string URL_TEXT = "IlluminaHiSeq pancan normalized";
        private const string DOWNLOAD_URL_SUBSTRING = "sampleMap%";
        private const string URLS_PATH = "download_urls.txt";
        private const string FILES_DOWNLOAD_PATH = "scans\\";
        private const string FIRST_LEVEL_URL = "https://xenabrowser.net/datapages/?hub=https://tcga.xenahubs.net:443";

        private static List<string> firstLevelUrls = new List<string>();
        private static List<string> secondLevelUrls = new List<string>();
        private static List<string> downloadUrls = new List<string>();

        static async Task Main(string[] args)
        {
            ScrapeWeb();
            LoadUrlsFromFile();
            await DownloadFiles();
        }

        private static void LoadUrlsFromFile()
        {
            File.ReadLines(URLS_PATH).ToList().ForEach(downloadUrls.Add);
        }

        private static async Task DownloadFiles()
        {
            Directory.CreateDirectory(FILES_DOWNLOAD_PATH);

            using (HttpClient client = new HttpClient())
            {
                int counter = 1;
                foreach (string url in downloadUrls)
                {
                    string fileName = Path.GetFileName(new Uri(url).LocalPath);

                    string uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{counter}{Path.GetExtension(fileName)}";

                    string filePath = Path.Combine(FILES_DOWNLOAD_PATH, uniqueFileName);

                    try
                    {
                        byte[] fileBytes = await client.GetByteArrayAsync(url);
                        await File.WriteAllBytesAsync(filePath, fileBytes);
                    }
                    catch (TaskCanceledException e)
                    {
                        Console.WriteLine($"Download timeout. Exception: {e.Message}");
                    }

                    Console.WriteLine($"Downloaded: {uniqueFileName}");

                    counter++;
                }
            }
        }

        private static void ScrapeWeb()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMicroseconds(0));

                FillFirstLevelUrls(driver, wait);
                Console.WriteLine("First level urls added.\n");

                FillSecondLevelUrls(driver, wait);
                Console.WriteLine("Second level urls added.\n");

                FillThirtLevelUrls(driver, wait);
                Console.WriteLine("Download urls added.\n");
            }

            Console.WriteLine("Saving to file...");
            SaveToFile();
        }

        private static void FillFirstLevelUrls(IWebDriver driver, WebDriverWait wait) 
        {
            driver.Navigate().GoToUrl(FIRST_LEVEL_URL);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
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
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(By.CssSelector(".Datapages-module__groupList___1h_hi")));

                try
                {
                    var aTag = driver.FindElement(By.LinkText(URL_TEXT));
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

        private static void FillThirtLevelUrls(IWebDriver driver, WebDriverWait wait)
        {
            foreach (var url in secondLevelUrls)
            {
                driver.Navigate().GoToUrl(url);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
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

        private static void SaveToFile()
        {
            System.IO.File.WriteAllLines(URLS_PATH, downloadUrls);
        }
    }
}
