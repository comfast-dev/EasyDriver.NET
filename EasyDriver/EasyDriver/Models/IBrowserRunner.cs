using OpenQA.Selenium;

namespace Comfast.EasyDriver.Models;

/// <summary> Implement it to customize running browser logic</summary>
public interface IBrowserRunner {
    /// <summary>
    /// This method will be used to create new Browser when need.
    /// Example:
    /// IWebDriver RunNewBrowser() { return new Chromedriver(); }
    /// </summary>
    IWebDriver RunNewBrowser();
}