using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra.Browser;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se.Infra;

/// <summary>
/// Return one Driver instance per thread. Configuration parameters:<br/>
/// - reconnect - try to recreate driver based on previous session<br/>
/// - autoClose - close browser after process exit
/// </summary>
public class DriverProvider : IDriverProvider {
    private readonly ThreadLocal<IWebDriver> _instances;
    private readonly DriverSessionStore _driverSessionStore = new();
    private readonly BrowserConfig _browserConfig;

    /// <summary>
    /// Update this field to customize browser creation logic
    /// </summary>
    public IBrowserRunner BrowserRunner { get; set; }

    /// <summary>
    /// Create new instance based on config.
    /// </summary>
    public DriverProvider(BrowserConfig browserConfig) {
        _instances = new ThreadLocal<IWebDriver>(ProvideDriverInstance, true);
        _browserConfig = browserConfig;
        BrowserRunner = new BrowserRunner(browserConfig);

        AppDomain.CurrentDomain.ProcessExit += (s, e) => {
            if (_browserConfig.AutoClose) CloseAllDrivers();
        };
    }

    /// <summary>
    /// Provide WebDriver Instance.<br/>
    /// - One per thread.<br/>
    /// - Can be called multiple times.
    /// </summary>
    public IWebDriver GetDriver() {
        //@formatter:off
        if (_browserConfig.Reconnect && _instances.Values.Count > 1) throw new Exception(@"
Reconnect feature isn't compatible with parallel runs. Possible solutions:
- Set BrowserConfig.Reconnect = false in EasyDriverConfig.json
- Change runner configuration to run tests in one thread");
        //@formatter:on
        return _instances.Value!;
    }

    /// <summary>
    /// Close all WebDrivers managed by this provider
    /// </summary>
    public void CloseAllDrivers() {
        Parallel.ForEach(_instances.Values, driver => driver.Quit());
    }

    /// <summary>
    /// Run/reconnect to Browser instance
    /// </summary>
    private IWebDriver ProvideDriverInstance() {
        return _browserConfig.Reconnect
            ? _driverSessionStore.RestoreSessionOrElse(BrowserRunner.RunNewBrowser)
            : BrowserRunner.RunNewBrowser();
    }
}