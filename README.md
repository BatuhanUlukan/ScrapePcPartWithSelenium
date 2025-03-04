# PC Part Picker Scraper 🖥️🔍

This console application extracts product specifications from **PCPartPicker** product pages. It reads product links from a `.txt` file, navigates to each page using Selenium WebDriver, and retrieves detailed specifications.

## 📌 Features
- Reads product links from a `txt` file.
- Navigates to each product page using Selenium WebDriver.
- Extracts specifications such as:
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
- Saves the extracted data into a structured format (CSV, JSON, or SQL-ready format).

## 🛠️ Prerequisites
Before running the application, ensure you have:
- **Google Chrome** installed.
- **Chrome WebDriver** matching your Chrome version.
- **Selenium WebDriver for C#** installed via NuGet:
  ```sh
  dotnet add package Selenium.WebDriver
