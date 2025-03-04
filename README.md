# PC Part Picker Scraper 🖥️🔍

This console application is designed to extract product specifications from **PCPartPicker** product pages. It reads product links from a `.txt` file, navigates to each page using Selenium WebDriver, and retrieves detailed specifications.

## 📌 Features
- Reads product links from a `txt` file.
- Navigates to each product page using Selenium WebDriver.
- Extracts specifications such as brand, part number, socket type, core count, clock speed, and other relevant details.
- Saves the extracted data into a structured format (CSV, JSON, or SQL-ready format).

## 🛠️ Prerequisites
Before running the application, ensure you have:
- **Google Chrome** installed.
- **Chrome WebDriver** matching your Chrome version.
- **Selenium WebDriver for C#** installed via NuGet:
  ```sh
  dotnet add package Selenium.WebDriver
