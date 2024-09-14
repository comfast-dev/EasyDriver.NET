using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se.Finder;

/// <summary>
/// Utility search methods for WebDriver instances
/// </summary>
public static class DriverExtensions {
    public static IWebElement Find(this IWebDriver driver, string cssOrXpath) =>
        new WebElementFinder(driver, cssOrXpath).Find();

    public static IList<IWebElement> FindAll(this IWebDriver driver, string cssOrXpath) =>
        new WebElementFinder(driver, cssOrXpath).FindAll();

    /// <summary>
    /// Executes JavaScript from browser console.
    /// </summary>
    public static T ExecuteJs<T>(this IWebDriver driver, string jsCode, params object[] args) {
        var jsDriver = (IJavaScriptExecutor)driver;
        return (T)jsDriver.ExecuteScript(jsCode, args);
    }
}