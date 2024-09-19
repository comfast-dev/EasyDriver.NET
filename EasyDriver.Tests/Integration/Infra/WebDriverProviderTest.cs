using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra;
using Comfast.EasyDriver.Ui;
using EasyDriver.Tests.Util;
using FluentAssertions;
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

        driver2.Title.Should().Be(originalTitle);
        driver2.ReadField<string>("SessionId.sessionOpaqueKey").Should().Be(sessionId);
    }

    [Fact] public void CustomBrowserRunnerTest() {
        //prepare browser
        const string pageTitle = "Lol page";
        var options = new ChromeOptions { BinaryLocation = Configuration.BrowserConfig.BrowserPath };
        options.AddArgument("headless");
        var myChrome = new ChromeDriver(Configuration.BrowserConfig.DriverPath, options);
        _browserContent.SetBody($"<h1>{pageTitle}</h1>");
        Configuration.BrowserConfig.AutoClose = true;

        //set configuration
        DriverProvider.SetCustomBrowser(() => myChrome);

        //assert
        ShouldHaveText(S("h1"), pageTitle);

        // close browser
        myChrome.Close();
        myChrome.Dispose();
    }

    [Fact] public void InvalidBrowserPath() {
        var conf = Configuration.BrowserConfig.Copy();
        conf.BrowserPath = "xd";

        var provider = new WebDriverProvider(conf);

        ShouldThrow(() => provider.GetDriver(), "Not found file path:" );
    }
}