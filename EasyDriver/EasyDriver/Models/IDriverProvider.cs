using OpenQA.Selenium;

namespace Comfast.EasyDriver.Models;

/// <summary> Implement this interface and put in Configuration to control WebDriver creation.</summary>
public interface IDriverProvider {
    /// <summary> Return WebDriver instance. Can be called many times. </summary>
    IWebDriver GetDriver();
}