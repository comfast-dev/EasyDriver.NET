using Comfast.EasyDriver.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace Comfast.EasyDriver.Se.Infra;

/// <summary>
/// Return one Driver instance per thread. Configuration parameters:
/// - reconnect - try to recreate driver based on previous session
/// - autoClose - close browser after process exit
/// </summary>
public class DriverProvider : IDriverProvider {
    private readonly ThreadLocal<WebDriver> _instances;
    private readonly DriverConfig _driverConfig;

    public DriverProvider(DriverConfig driverConfig) {
        _instances = new ThreadLocal<WebDriver>(ProvideDriverInstance, true);
        _driverConfig = driverConfig;
    }

    /// <summary>
    /// Provide WebDriver Instance.
    /// - One per thread.
    /// - Can be called multiple times.
    /// </summary>
    public IWebDriver GetDriver() {
        if (_driverConfig.Reconnect && _instances.Values.Count > 1) {
            throw new Exception(@"
Reconnect feature isn't compatible with parallel runs. Possible solutions:
- Set DriverConfig.Reconnect flag to false in AppConfig.json
- Change runner configuration to run tests in one thread
");
        }

        return _instances.Value!;
    }

    /// <summary>
    /// Run/reconnect to Browser instance
    /// </summary>
    private WebDriver ProvideDriverInstance() {
        bool reconnect = _driverConfig.Reconnect;
        bool autoClose = _driverConfig.AutoClose;

        var driver = reconnect && !autoClose
            ? DriverSessionStore.RestoreSessionOrElse(RunNewDriver)
            : RunNewDriver();

        if (autoClose) AddShutdownHook(driver);

        return driver;
    }

    /// <summary>
    /// Make sure WebDriver is closed after end of process
    /// </summary>
    private void AddShutdownHook(IWebDriver driver) {
        AppDomain.CurrentDomain.ProcessExit += (s, e) => {
            driver.Close(); // closes browser
            driver.Dispose(); // closes driver process ( e.g. chromedriver.exe )
        };
    }

    /// <summary>
    ///  Run new WebDriver instance
    /// </summary>
    private WebDriver RunNewDriver() {
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
        var opts = new ChromeOptions();
        if (_driverConfig.Headless) opts.AddArguments("headless");
        opts.BinaryLocation = _driverConfig.BrowserPath;

        try {
            return new ChromeDriver(_driverConfig.DriverPath, opts);
        } catch (InvalidOperationException e) {
            if (e.Message.Contains("No process is associated with this object."))
                throw new Exception("Invalid driver path: " + Configuration.DriverConfig.DriverPath, e);
            throw;
        }
    }

    private FirefoxDriver RunFirefox() {
        var opts = new FirefoxOptions();
        if (_driverConfig.Headless) opts.AddArguments("--headless");
        opts.BinaryLocation = _driverConfig.BrowserPath;

        return new FirefoxDriver(opts);
    }

    private EdgeDriver RunEdge() {
        var opts = new EdgeOptions();
        if (_driverConfig.Headless) opts.AddArguments("headless");
        opts.BinaryLocation = _driverConfig.BrowserPath;
        return new EdgeDriver(_driverConfig.DriverPath, opts);
    }
}