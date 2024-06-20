using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra;
using Comfast.EasyDriver.Se.Infra.Browser;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

/// <summary>
/// Main source of truth for framework.
/// Internal fields of DriverConfig/LocatorConfig can be edited in runtime.
/// </summary>
public static class Configuration {
    /// <summary>
    /// Options that define way how WebDriver managed browser is created/managed
    /// </summary>
    public static DriverConfig DriverConfig { get; }

    /// <summary>
    /// Feature flags / timeouts for locators
    /// </summary>
    public static LocatorConfig LocatorConfig { get; }

    /// <summary>
    /// Main logic that manages WebDriver creation
    /// </summary>
    public static DriverProvider DriverProvider { get; private set; }

    static Configuration() {
        //initialize
        DriverConfig = ConfigLoader.Load<DriverConfig>("AppConfig.json", "DriverConfig");
        LocatorConfig = ConfigLoader.Load<LocatorConfig>("AppConfig.json", "LocatorConfig");
        DriverProvider = new DriverProvider(DriverConfig);
    }

    /// <summary>
    /// Gives ability to override way that WebDriver is created.
    /// Keeps Reconnect, AutoClose and Multi-thread handling untouched.
    /// </summary>
    /// <param name="runBrowser">Function that will run browser e.g. () => new ChromeDriver(myOptions)</param>
    public static void SetCustomBrowser(Func<IWebDriver> runBrowser) {
        DriverProvider.BrowserRunner = new SimpleBrowserRunner(runBrowser);
    }

    /// <summary>
    /// Override DriverProvider including Reconnect, AutoClose and Multi-thread handling.
    /// Not recommended, suggest to use "SetCustomBrowser" method.
    /// </summary>
    public static void SetCustomDriverProvider(DriverProvider driverProvider) {
        DriverProvider = driverProvider;
    }
}