using Comfast.EasyDriver.Core.Infra.Browser;
using Comfast.EasyDriver.Models;
using OpenQA.Selenium;
using static Comfast.EasyDriver.Core.Infra.Browser.WebDriverReconnectUtils;

namespace Comfast.EasyDriver.Core.Infra;

/// <summary>
/// Return one Driver instance per thread. Configuration parameters:<br/>
/// - reconnect - try to recreate driver based on previous session<br/>
/// - autoClose - close browser after process exit
/// </summary>
public class WebDriverProvider : IWebDriverProvider {
    private readonly ThreadLocal<IWebDriver> _instances;
    private readonly TempFile _sessionFile;
    private readonly BrowserConfig _browserConfig;
    private IBrowserRunner _browserRunner;

    /// <summary> Create new instance based on config.</summary>
    public WebDriverProvider(BrowserConfig browserConfig) {
        _instances = new ThreadLocal<IWebDriver>(ProvideDriverInstance, true);
        _sessionFile = new("EasyDriver/WebDriverSessionInfo.txt");
        _browserConfig = browserConfig;
        _browserRunner = new BrowserRunner(browserConfig);

        AppDomain.CurrentDomain.ProcessExit += (s, e) => {
            if (_browserConfig.AutoClose) CloseAllDrivers();
        };
    }

    /// <summary> Provide WebDriver instance </summary>
    public IWebDriver GetDriver() {
        if (_browserConfig.Reconnect && _instances.Values.Count > 1) {
            throw new Exception(@"Reconnect feature isn't compatible with parallel runs. Possible solutions:
- Set BrowserConfig.Reconnect = false in EasyDriverConfig.json
- Change runner configuration to run tests in one thread");
        }

        return _instances.Value!;
    }

    /// <summary> Close all WebDrivers managed by this provider</summary>
    public void CloseAllDrivers() {
        Parallel.ForEach(_instances.Values, driver => driver.Dispose());
    }

    /// <summary>
    /// Overrides how WebDriver is created.
    /// Doesn't affect to Reconnect, AutoClose and Multi-thread functionalities.
    /// Given function "runBrowser" will be called once to create WebDriver per every thread
    /// </summary>
    /// <param name="runBrowser">Function that will run browser e.g. () => new ChromeDriver(myOptions)</param>
    public void SetCustomBrowser(Func<IWebDriver> runBrowser) {
        _browserRunner = new SimpleBrowserRunner(runBrowser);
    }

    /// <summary> Run/reconnect to Browser instance</summary>
    private IWebDriver ProvideDriverInstance() {
        if (_browserConfig.Reconnect && _sessionFile.Exists) {
            var sessionInfo = _sessionFile.ReadFile();
            var reconnectedDriver = CreateDriver(sessionInfo);
            if (reconnectedDriver.TestConnection()) return reconnectedDriver;
        } //in case of fail reconnection run new Browser

        var newDriver = _browserRunner.RunNewBrowser();
        var newBrowserSessionString = ExtractSessionString(newDriver);
        if (_browserConfig.Reconnect) _sessionFile.SaveFile(newBrowserSessionString);

        return CreateDriver(newBrowserSessionString);
    }
}