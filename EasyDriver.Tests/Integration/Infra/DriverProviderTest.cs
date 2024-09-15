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

public class DriverProviderTest : IntegrationBase, IDisposable {
    public DriverProviderTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    private readonly string _driverPath = EasyDriverConfig.BrowserConfig.DriverPath;
    private readonly string _browserPath = EasyDriverConfig.BrowserConfig.BrowserPath;
    private IWebDriver? _currentDriver;

    public void Dispose() {
        EasyDriverConfig.BrowserConfig.DriverPath = _driverPath;
        EasyDriverConfig.BrowserConfig.BrowserPath = _browserPath;
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
            BrowserPath = EasyDriverConfig.BrowserConfig.BrowserPath,
            DriverPath = EasyDriverConfig.BrowserConfig.DriverPath,
        };

        var driverProvider1 = new DriverProvider(conf);
        var driver1 = driverProvider1.GetDriver();
        _currentDriver = driver1;
        var originalTitle = driver1.Title;
        var sessionId = driver1.ReadField<string>("SessionId.sessionOpaqueKey");

        // simulates new Process trying to reconnect to the same driver
        var driverProvider2 = new DriverProvider(conf);
        var driver2 = driverProvider2.GetDriver();

        driver2.Title.Should().Be(originalTitle);
        driver2.ReadField<string>("SessionId.sessionOpaqueKey").Should().Be(sessionId);
    }

    [Fact] public void CustomBrowserRunnerTest() {
        //prepare browser
        const string pageTitle = "Lol page";
        var options = new ChromeOptions { BinaryLocation = EasyDriverConfig.BrowserConfig.BrowserPath };
        options.AddArgument("headless");
        var myChrome = new ChromeDriver(EasyDriverConfig.BrowserConfig.DriverPath, options);
        _browserContent.SetBody($"<h1>{pageTitle}</h1>");
        EasyDriverConfig.BrowserConfig.AutoClose = true;

        //set configuration
        EasyDriverConfig.SetCustomBrowser(() => myChrome);

        //assert
        ShouldHaveText(S("h1"), pageTitle);

        // close browser
        myChrome.Close();
        myChrome.Dispose();
    }

    // [Fact] public void InvalidBrowserPath() {
    //     EasyDriverConfig.BrowserConfig.BrowserPath = "lol";
    //
    //     var action = () => DriverApi.Driver;
    //     action.Should().Throw<Exception>()
    //         .WithMessage("unknown error: no chrome binary at lol");
    // }
}