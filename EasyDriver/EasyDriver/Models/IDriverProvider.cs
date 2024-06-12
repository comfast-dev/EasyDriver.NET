using OpenQA.Selenium;

namespace Comfast.EasyDriver.Models;

public interface IDriverProvider {
    /// <summary>
    /// Return WebDriver instance
    /// </summary>
    IWebDriver GetDriver();
}