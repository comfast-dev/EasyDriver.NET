using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Locator;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

/// <summary>
/// Main API of framework
/// </summary>
public static class DriverApi {
    /// <summary>
    /// Returns current WebDriver instance.
    /// </summary>
    public static IWebDriver GetDriver() => Configuration.DriverProvider.GetDriver();

    /// <summary>
    /// Shortcut to navigation
    /// </summary>
    public static void NavigateTo(string url) => GetDriver().Url = url;

    /// <summary>
    /// Creates locator instance
    /// </summary>
    /// <param name="cssOrXpath">Any selector</param>
    /// <param name="description"></param>
    public static ILocator S(string cssOrXpath, string description = "") => new SimpleLocator(cssOrXpath, description);

    /// <summary>
    /// Executes JavaScript from browser console.
    /// </summary>
    public static T ExecuteJs<T>(string jsCode, params object[] args) {
        var jsDriver = (IJavaScriptExecutor)GetDriver();
        return (T)jsDriver.ExecuteScript(jsCode, args);
    }

    /// <summary>
    /// Wait for any action to return true. Ignore Exceptions.
    /// </summary>
    public static void WaitFor(Func<bool> action, string? description = null, int? timeoutMs = null) {
        WaitUtils.WaitFor(action, description, timeoutMs);
    }
}