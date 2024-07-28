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
    private readonly DriverConfig _driverConfig;
    private readonly DriverSessionStore _driverSessionStore = new();

    /// <summary>
    /// Update this field to customize browser creation logic
    /// </summary>
    public IBrowserRunner BrowserRunner { get; set; }

    public DriverProvider(DriverConfig driverConfig) {
        _instances = new ThreadLocal<IWebDriver>(ProvideDriverInstance, true);
        _driverConfig = driverConfig;
        BrowserRunner = new BrowserRunner(driverConfig);

        AppDomain.CurrentDomain.ProcessExit += (s, e) => {
            if (_driverConfig.AutoClose) CloseAllDrivers();
        };
    }

    /// <summary>
    /// Provide WebDriver Instance.<br/>
    /// - One per thread.<br/>
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
    private IWebDriver ProvideDriverInstance() {
        return _driverConfig.Reconnect
            ? _driverSessionStore.RestoreSessionOrElse(BrowserRunner.RunNewBrowser)
            : BrowserRunner.RunNewBrowser();
    }

    /// <summary>
    /// Close all WebDrivers managed by this provider
    /// </summary>
    public void CloseAllDrivers() {
        foreach (var webDriver in _instances.Values)
            webDriver.Quit();
    }
}