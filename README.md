# 🖥️ PC Part Picker Scraper 🔍

This console application extracts product specifications from **PCPartPicker** product pages. It reads product links from a `.txt` file, navigates to each page using Selenium WebDriver, and retrieves detailed specifications.

---

## 📌 Features
✅ Reads product links from a `.txt` file.
✅ Navigates to each product page using Selenium WebDriver.
✅ Extracts detailed product specifications:
  - **Manufacturer**
  - **Part Number**
  - **Microarchitecture**
  - **Socket Type**
  - **Core/Thread Count**
  - **Base & Boost Clock Speed**
  - **L2 & L3 Cache**
  - **TDP (Thermal Design Power)**
  - **Integrated Graphics Support**
  - **Memory Compatibility**
✅ Saves the extracted data in structured formats: **CSV, JSON, or SQL-ready format**.

---

## 🛠️ Prerequisites
Before running the application, ensure you have:
- **Google Chrome** installed.
- **Chrome WebDriver** matching your Chrome version.
- **Selenium WebDriver for C#** installed via NuGet:
  ```sh
  dotnet add package Selenium.WebDriver
  ```  

---

## 🚀 How to Run
1. **Download or Clone the Scraper**  
   You can get the scraper from [this repository](https://github.com/BatuhanUlukan/ScrapeLinks), which is a console application that extracts links from a `.txt` file.  
2. **Run the Console Application**  
   Execute the scraper to extract links from the `.txt` file and gather product details from **PCPartPicker**.  
3. **View and Save the Data**  
   The extracted data will be available in your preferred format.

---

📂 Latest JSON Data (Updated: 02.02.2025)

🔗 You can find the latest scraped JSON files here:
👉 Scraped JSON Files [ScrapedProducts Jsons]([https://github.com/BatuhanUlukan/ScrapeLinks](https://github.com/BatuhanUlukan/ScrapePcPartWithSelenium/tree/master/ConsoleApp1/DetailJsons)) 

🔗 **Get Started Now:** [ScrapeLinks Repository](https://github.com/BatuhanUlukan/ScrapeLinks) 🚀

