﻿using System.Text.RegularExpressions;
using Comfast.EasyDriver.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace Comfast.EasyDriver.Core.Infra.Browser;

/// <summary> Covers logic of running different browsers.</summary>
public class BrowserRunner : IBrowserRunner {
    private readonly BrowserConfig _config;

    public BrowserRunner(BrowserConfig browserConfig) {
        _config = browserConfig;
    }

    /// <summary>  Run new WebDriver instance</summary>
    public IWebDriver RunNewBrowser() {
        AssertFileExists(_config.DriverPath, "BrowserConfig.DriverPath");
        AssertFileExists(_config.BrowserPath, "BrowserConfig.BrowserPath");
        // AssertLocation(_config.DownloadPath, "BrowserConfig.DownloadPath");
        if (_config.ProxyUrl != null) AssertUrl(_config.ProxyUrl, "ProxyUrl");

        var browserName = _config.BrowserName;
        return browserName switch {
            "chrome" or "chromium" or "brave" => RunChromium(),
            "firefox" => RunFirefox(),
            "edge" => RunEdge(),
            _ => throw new($"Invalid browser name: {browserName}. Check your configuration.")
        };
    }

    private void AssertUrl(string url, string title) {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new(
                $"Invalid URL: {title}: {url}");
    }

    private void AssertFileExists(string filePath, string fieldName) {
        if (!File.Exists(filePath))
            throw new($@"Not found file path: {fieldName}: '{filePath}'. Check your configuration.");
    }

    private ChromeDriver RunChromium() {
        var options = new ChromeOptions();

        //proxy
        if (_config.ProxyUrl != null) options.AddArgument($"--proxy-server=" + _config.ProxyUrl);

        //download
        options.AddUserProfilePreference("download.default_directory", _config.DownloadPath);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("download.directory_upgrade", true);
        options.AddUserProfilePreference("safebrowsing.enabled", true);

        //headless
        if (_config.Headless) options.AddArgument("headless");

        // turns off info bars
        // options.AddExcludedArgument("enable-automation"); //infobar 1
        options.AddArgument("--disable-infobars");

        //window size
        var match = ValidateWindowSize();
        if (match.Value == "default") { }
        else if (match.Value == "maximized") options.AddArgument("--start-maximized");
        else options.AddArgument($"--window-size={match.Groups[1]},{match.Groups[2]}");

        options.BinaryLocation = _config.BrowserPath;
        return new ChromeDriver(_config.DriverPath, options);
    }

    private FirefoxDriver RunFirefox() {
        var options = new FirefoxOptions();

        //downloads
        options.SetPreference("browser.download.folderList", 2);
        options.SetPreference("browser.download.dir", _config.DownloadPath);
        options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/pdf,application/octet-stream");

        //headless
        if (_config.Headless) options.AddArguments("--headless");

        //window size
        var match = ValidateWindowSize();
        if (match.Value == "default") { }
        else if (match.Value == "maximized") options.AddArgument("--start-maximized");
        else {
            options.AddArgument($"--width={match.Groups[1]}");
            options.AddArgument($"--height={match.Groups[2]}");
        }

        options.BinaryLocation = _config.BrowserPath;
        return new FirefoxDriver(_config.DriverPath, options);
    }

    private EdgeDriver RunEdge() {
        var options = new EdgeOptions();

        //download
        options.AddUserProfilePreference("download.default_directory", _config.DownloadPath);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
        options.AddUserProfilePreference("safebrowsing.enabled", true);

        //headless
        if (_config.Headless) options.AddArguments("headless");

        //window size
        var match = ValidateWindowSize();
        if (match.Value == "default") { }
        else if (match.Value == "maximized") {
            options.AddArgument("--start-maximized");
        } else {
            options.AddArgument($"--window-size={match.Groups[1]},{match.Groups[2]}");
        }

        options.BinaryLocation = _config.BrowserPath;
        return new EdgeDriver(_config.DriverPath, options);
    }

    /// <summary> Validate WindowSize variable</summary>
    /// <returns>validated match</returns>
    private Match ValidateWindowSize() {
        var size = _config.WindowSize;
        var match = Regex.Match(size ?? "", @"(\d+)[x\- ,](\d+)|default|maximized");
        if (!match.Success) throw new($"Invalid WindowSize='{size}', accepted are: 1234x567 | default | maximized");
        return match;
    }
}