using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCGA_Scrapper.Utilities
{
    public static class ScrapperUtils
    {
        public static IList<string> GetUrlsFromUrls(
            IWebDriver driver, 
            WebDriverWait wait, 
            By by, 
            By waitBy, 
            IEnumerable<string> urls, 
            string errorMsg)
        {
            IList<string> newUrls = new List<string>();

            foreach (var url in urls)
            {
                driver.Navigate().GoToUrl(url);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(waitBy));

                try
                {
                    var aTag = driver.FindElement(by);
                    string href = aTag.GetAttribute("href");
                    newUrls.Add(href);
                    Console.WriteLine($"Added url: {href}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(errorMsg + ex.Message + '\n');
                }
            }

            return newUrls;
        }

        public static IList<string> GetUrlsFromUrl(
            IWebDriver driver,
            WebDriverWait wait,
            By by,
            By elementsBy,
            string url)
        {
            IList<string> newUrls = new List<string>();

            driver.Navigate().GoToUrl(url);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElements(elementsBy).Count >= 6);

            var listItems = driver.FindElements(elementsBy);
            foreach (var item in listItems)
            {
                var aTag = item.FindElement(by);
                string href = aTag.GetAttribute("href");
                newUrls.Add(href);
                Console.WriteLine($"Added url: {href}\n");
            }

            return newUrls;
        }
    }
}
