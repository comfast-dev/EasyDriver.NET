using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra;
using Comfast.EasyDriver.Se.Infra.Browser;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

/// <summary>
/// Main source of truth for framework.
/// Internal fields of BrowserConfig/RuntimeConfig can be edited in runtime.
/// </summary>
public static class EasyDriverConfig {
    /// <summary>
    /// Options that define way how WebDriver managed browser is created/managed
    /// </summary>
    public static BrowserConfig BrowserConfig { get; } = new();

    /// <summary>
    /// Feature flags / timeouts
    /// </summary>
    public static RuntimeConfig RuntimeConfig { get; } = new();

    /// <summary>
    /// Main logic that manages WebDriver creation
    /// </summary>
    public static DriverProvider DriverProvider { get; private set; }

    static EasyDriverConfig() {
        ReloadConfig("EasyDriverConfig.json");
        DriverProvider = new(BrowserConfig);
    }

    /// <summary>
    /// Overrides how WebDriver is created.
    /// Doesn't affect to Reconnect, AutoClose and Multi-thread functionalities.
    /// Given function "runBrowser" will be called once to create WebDriver per every thread
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
        DriverProvider.CloseAllDrivers();
        DriverProvider = driverProvider;
    }

    /// <summary>
    /// Reloads Configuration from given Config file.
    /// </summary>
    /// <param name="filePath"> e.g. MyAppConfig.json</param>
    public static void ReloadConfig(string filePath) {
        ReloadConfig(new ConfigurationBuilder().AddJsonFile(filePath).Build());
    }

    /// <summary>
    /// Reloads configuration.
    /// </summary>
    /// <param name="conf">IConfiguration object</param>
    public static void ReloadConfig(IConfiguration conf) {
        // todo if DriverProvider.Instances.Count > 0 - show warning/throw

        BrowserConfig.RewriteFrom(conf.GetSection("BrowserConfig").Get<BrowserConfig>());
        RuntimeConfig.RewriteFrom(conf.GetSection("RuntimeConfig").Get<RuntimeConfig>());
    }
}