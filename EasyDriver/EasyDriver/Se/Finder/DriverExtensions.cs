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
}