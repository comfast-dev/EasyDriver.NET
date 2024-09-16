using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Locator;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

/// <summary> Main API of framework</summary>
public static class EasyDriverApi {
    /// <summary> Returns current WebDriver instance.</summary>
    public static IWebDriver GetDriver() => EasyDriverConfig.DriverProvider.GetDriver();

    /// <see cref="Locator"/>
    public static ILocator S(string cssOrXpath, string description = "Locator") => new SimpleLocator(cssOrXpath, description);

    /// <summary> Creates locator instance</summary>
    /// <param name="cssOrXpath">Any selector</param>
    /// <param name="description"></param>
    public static ILocator Locator(string cssOrXpath, string description = "Locator") => new SimpleLocator(cssOrXpath, description);

}