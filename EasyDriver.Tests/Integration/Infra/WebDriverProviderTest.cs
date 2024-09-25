using Comfast.Commons.Utils;
using Comfast.EasyDriver.Core.Infra;
using Comfast.EasyDriver.Lib;
using Comfast.EasyDriver.Models;
using EasyDriver.Tests.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration.Infra;

public class WebDriverProviderTest : IntegrationBase, IDisposable {
    public WebDriverProviderTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    private readonly string _driverPath = Configuration.BrowserConfig.DriverPath;
    private readonly string _browserPath = Configuration.BrowserConfig.BrowserPath;
    private IWebDriver? _currentDriver;

    public void Dispose() {
        Configuration.BrowserConfig.DriverPath = _driverPath;
        Configuration.BrowserConfig.BrowserPath = _browserPath;
        _currentDriver?.Quit();
        _currentDriver?.Dispose();
    }

    // [Fact]
    [Fact(Skip = "Need to run separately")]
    public void ReconnectToSessionTest() {
        var conf = new BrowserConfig {
            Reconnect = true,
            Headless = true,
            AutoClose = true,
            BrowserPath = Configuration.BrowserConfig.BrowserPath,
            DriverPath = Configuration.BrowserConfig.DriverPath,
        };

        var driverProvider1 = new WebDriverProvider(conf);
        var driver1 = driverProvider1.GetDriver();
        _currentDriver = driver1;
        var originalTitle = driver1.Title;
        var sessionId = driver1.ReadField<string>("SessionId.sessionOpaqueKey");

        // simulates new Process trying to reconnect to the same driver
        var driverProvider2 = new WebDriverProvider(conf);
        var driver2 = driverProvider2.GetDriver();

        Assert.Equal(originalTitle, driver2.Title);
        Assert.Equal(sessionId, driver2.ReadField<string>("SessionId.sessionOpaqueKey"));
    }

    [Fact] public void CustomBrowserRunnerTest() {
        var browserConfig = Configuration.BrowserConfig.Copy();
        var driverProvider = new WebDriverProvider(browserConfig);

        //prepare browser
        const string pageTitle = "Lol page";
        var options = new ChromeOptions { BinaryLocation = browserConfig.BrowserPath };
        options.AddArgument("headless");
        var myChrome = new ChromeDriver(browserConfig.DriverPath, options);

        new BrowserContent(myChrome).SetBody($"<h1>{pageTitle}</h1>");
        browserConfig.AutoClose = true;

        //set configuration
        driverProvider.SetCustomBrowser(() => myChrome);

        //assert
        Assert.Equal(pageTitle, myChrome.FindElement(By.CssSelector("h1")).Text);

        // close browser
        myChrome.Close();
        myChrome.Dispose();
    }

    [Fact] public void InvalidBrowserPath() {
        var conf = Configuration.BrowserConfig.Copy();
        conf.BrowserPath = "xd";
        conf.Reconnect = false;

        var provider = new WebDriverProvider(conf);

        ShouldThrow(() => provider.GetDriver(), "Not found file path:" );
    }
}