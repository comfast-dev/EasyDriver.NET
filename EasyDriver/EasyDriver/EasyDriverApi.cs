using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra;
using Comfast.EasyDriver.Se.Locator;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

/// <summary> Main API of framework</summary>
public static class EasyDriverApi {
    public static EasyDriverConfig Configuration { get; } = new();
    public static WebDriverProvider DriverProvider { get; } = new(Configuration.BrowserConfig);

    /// <summary> Returns current WebDriver instance.</summary>
    public static IWebDriver GetDriver() => DriverProvider.GetDriver();

    /// <see cref="GetLocator"/>
    public static ILocator S(string cssOrXpath, string description = "Locator") {
        return new SimpleLocator(cssOrXpath, description);
    }

    /// <summary>  </summary>
    /// <param name="cssOrXpath"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static ILocator GetLocator(string cssOrXpath, string description = "Locator") {
        return new SimpleLocator(cssOrXpath, description);
    }
}