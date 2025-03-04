using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Newtonsoft.Json;
using Cookie = OpenQA.Selenium.Cookie;

class Program
{
    static void Main(string[] args)
    {
        // Setting up Chrome WebDriver using WebDriverManager
        new DriverManager().SetUpDriver(new ChromeConfig());
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");

        // Load VPN extension to bypass potential restrictions
        options.AddArgument("--load-extension=C:\\Users\\Administrator\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Extensions\\omghfjlpggmjjaagoclmmobgdodcjboh\\3.88.4_0");//it's browsecvpn

        // Create a temporary Chrome user profile directory to avoid tracking
        var tempProfileDir = Path.Combine(Path.GetTempPath(), "SeleniumChromeProfile_" + Guid.NewGuid());
        Directory.CreateDirectory(tempProfileDir);
        options.AddArgument($"--user-data-dir={tempProfileDir}");

        using (var driver = new ChromeDriver(options))
        {
            // Read all product links from the file
            var links = File.ReadAllLines("links/storagejsonlinks.txt").ToList();
            var memoryData = new List<Dictionary<string, string>>();
            int processedCount = 0;
            int batchSize = 499; // Batch size set to 499 to avoid rate limits or server restrictions
            int batchNumber = 1;
            var linksToRemove = new List<string>();

            for (int i = 0; i < links.Count; i++)
            {
                // Every 10 links, refresh session cookies to simulate a new user session
                if (i % 10 == 0 && i != 0)
                {
                    Console.WriteLine("Refreshing cookies...");
                    driver.Manage().Cookies.DeleteAllCookies();
                    driver.Manage().Cookies.AddCookie(new Cookie("session", Guid.NewGuid().ToString()));
                    Console.WriteLine("New session cookies set.");
                }

                // Navigate to the product page
                driver.Navigate().GoToUrl(links[i]);
                Console.WriteLine($"{links[i]} loaded, waiting 1 second...");
                Thread.Sleep(1000);
                linksToRemove.Add(links[i]);

                try
                {
                    // Check if a verification page (captcha or bot detection) is triggered
                    var verificationElement = driver.FindElements(By.CssSelector("p#LLeF5.h2.spacer-bottom"));
                    if (verificationElement.Count > 0)
                    {
                        Console.WriteLine("Verification detected! Stopping process...");
                        SaveToJson(memoryData, batchNumber);
                        RemoveProcessedLinks(links, linksToRemove);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while checking verification: {ex.Message}");
                }

                // Extract product image
                string imageUrl = "";
                var imageElements = driver.FindElements(By.CssSelector("a.pp_open_gallery img"));
                if (imageElements.Count > 0)
                {
                    imageUrl = imageElements[0].GetAttribute("src");
                }

                // Handle missing images
                string miniImageUrl = string.IsNullOrWhiteSpace(imageUrl) || imageUrl == "/static/forever/img/no-image.png"
                                       ? "Content/images/no-image.webp"
                                       : imageUrl;
                string largeImageUrl = miniImageUrl.Replace("256p", "1600");

                Directory.CreateDirectory("images");

                // Extract product name and sanitize it for file naming
                string name = driver.FindElement(By.CssSelector("h1.pageTitle")).Text;
                name = name.Replace("|", "I");
                var invalidChars = Path.GetInvalidFileNameChars();
                string sanitizedName = string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

                // Define file paths for downloaded images
                string miniImagePath = $"images/mini_{sanitizedName.Replace(" ", "_").Replace("/", "-")}.jpg";
                string largeImagePath = $"images/large_{sanitizedName.Replace(" ", "_").Replace("/", "-")}.jpg";

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(miniImageUrl, miniImagePath);
                    client.DownloadFile(largeImageUrl, largeImagePath);
                }

                // Extract product specifications
                var specs = driver.FindElements(By.CssSelector("div.group.group--spec"));
                var productData = new Dictionary<string, string>
                {
                    { "name", name },
                    { "miniImage", miniImagePath },
                    { "largeImage", largeImagePath }
                };

                foreach (var spec in specs)
                {
                    if (spec.FindElements(By.CssSelector("div.lhs-benchmark-value")).Count > 0)
                    {
                        string benchmarkValue = spec.FindElement(By.CssSelector("div.lhs-benchmark-value"))
                                                   .Text.Split('(')[0].Trim();
                        string title = spec.FindElements(By.CssSelector("h3.group__title")).FirstOrDefault()?.Text ?? "Benchmark";
                        productData[title] = benchmarkValue;
                    }
                    else if (spec.FindElements(By.CssSelector("h3.group__title")).Count > 0)
                    {
                        string title = spec.FindElement(By.CssSelector("h3.group__title")).Text;
                        string value = spec.FindElement(By.CssSelector("div.group__content")).Text;
                        if (spec.FindElements(By.CssSelector("li")).Count > 0)
                        {
                            value = string.Join(", ", spec.FindElements(By.CssSelector("li")).Select(i => i.Text));
                        }
                        productData[title] = value;
                    }
                }

                memoryData.Add(productData);
                processedCount++;
                Console.WriteLine($"Data collected: {name}");

                if (processedCount % batchSize == 0)
                {
                    SaveToJson(memoryData, batchNumber);
                    memoryData.Clear();
                    RemoveProcessedLinks(links, linksToRemove);
                    Console.WriteLine("Continue? [Y/N]");
                    if (!Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase))
                        return;
                    batchNumber++;
                }

                Thread.Sleep(1000);
            }
            if (memoryData.Count > 0)
                SaveToJson(memoryData, batchNumber);
            RemoveProcessedLinks(links, linksToRemove);
        }
    }

    static void SaveToJson(List<Dictionary<string, string>> memoryData, int batchNumber)
    {
        if (memoryData.Count == 0) return;
        string json = JsonConvert.SerializeObject(memoryData, Formatting.Indented);
        string fileName = $"storage_details_{batchNumber}.json";
        File.WriteAllText(fileName, json);
        Console.WriteLine($"JSON kaydedildi: {fileName}");
    }

    static void RemoveProcessedLinks(List<string> links, List<string> linksToRemove)
    {
        links.RemoveAll(link => linksToRemove.Contains(link));
        File.WriteAllLines("links/storagejsonlinks.txt", links);
        Console.WriteLine($"{linksToRemove.Count} işlenen link dosyadan silindi ve güncellendi.");
    }
}
