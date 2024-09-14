using Comfast.EasyDriver.Se.Finder;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Ui;

public class BrowserContent {
    private readonly IWebDriver _driver;

    public BrowserContent(IWebDriver driver) {
        _driver = driver;
    }

    /// <summary>
    /// Copy given resource file to temp folder and open it in browser.
    /// </summary>
    /// <param name="resourcePath">path to resource file, where root is "src/test/resources"</param>
    public void OpenResourceFile(string resourcePath) {
        var path = Path.Combine(Directory.GetCurrentDirectory(), resourcePath);
        _driver.Url = path;
    }

    /// <summary>
    /// Set body tag content
    /// </summary>
    public void SetBody(string bodyHtml) {
        _driver.ExecuteJs<object>("document.querySelector('html>body').innerHTML = arguments[0]", bodyHtml);
    }
}