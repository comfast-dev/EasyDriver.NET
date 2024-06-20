using Comfast.EasyDriver.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace Comfast.EasyDriver.Se.Infra.Browser;

/// <summary>
/// Covers logic of running different browsers.
/// </summary>
public class BrowserRunner : IBrowserRunner {
    private readonly DriverConfig _driverConfig;
    public BrowserRunner(DriverConfig driverConfig) {
        _driverConfig = driverConfig;
    }

    /// <summary>
    ///  Run new WebDriver instance
    /// </summary>
    public IWebDriver RunNewBrowser() {
        var browser = _driverConfig.BrowserName;
        switch (browser) {
            case "chrome":
            case "chromium":
            case "brave":
                return RunChromium();
            case "firefox":
                return RunFirefox();
            case "edge":
                return RunEdge();
            default: throw new Exception("Invalid browser: " + browser);
        }
    }

    private ChromeDriver RunChromium() {
        var options = new ChromeOptions();

        //headless
        if (_driverConfig.Headless) options.AddArgument("headless");

        //download
        options.AddUserProfilePreference("download.default_directory", _driverConfig.DownloadPath);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("download.directory_upgrade", true);
        options.AddUserProfilePreference("safebrowsing.enabled", true);

        // turns off infobar
        // options.AddExcludedArgument("enable-automation"); //infobar 1
        options.AddArgument("--disable-infobars");

        options.BinaryLocation = _driverConfig.BrowserPath;

        //proper error message
        try {
            return new ChromeDriver(_driverConfig.DriverPath, options);
        } catch (InvalidOperationException e) {
            if (e.Message.Contains("No process is associated with this object."))
                throw new Exception("Invalid driver path: " + Configuration.DriverConfig.DriverPath, e);
            throw;
        }
    }

    private FirefoxDriver RunFirefox() {
        var options = new FirefoxOptions();
        //downloads
        options.SetPreference("browser.download.folderList", 2);
        options.SetPreference("browser.download.dir", _driverConfig.DownloadPath);
        options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/pdf,application/octet-stream");

        //headless
        if (_driverConfig.Headless) options.AddArguments("--headless");

        options.BinaryLocation = _driverConfig.BrowserPath;
        return new FirefoxDriver(options);
    }

    private EdgeDriver RunEdge() {
        var options = new EdgeOptions();

        //download
        options.AddUserProfilePreference("download.default_directory", _driverConfig.DownloadPath);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
        options.AddUserProfilePreference("safebrowsing.enabled", true);

        //headless
        if (_driverConfig.Headless) options.AddArguments("headless");

        options.BinaryLocation = _driverConfig.BrowserPath;
        return new EdgeDriver(_driverConfig.DriverPath, options);
    }
}