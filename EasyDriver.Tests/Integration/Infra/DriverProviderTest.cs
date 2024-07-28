using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra;
using Comfast.EasyDriver.Ui;
using EasyDriver.Tests.Util;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit.Abstractions;
using Configuration = Comfast.EasyDriver.Configuration;

namespace EasyDriver.Tests.Integration.Infra;

public class DriverProviderTest : IntegrationBase, IDisposable {
    public DriverProviderTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    private readonly string _driverPath = Configuration.DriverConfig.DriverPath;
    private readonly string _browserPath = Configuration.DriverConfig.BrowserPath;
    private IWebDriver? _driver;

    public void Dispose() {
        Configuration.DriverConfig.DriverPath = _driverPath;
        Configuration.DriverConfig.BrowserPath = _browserPath;
        _driver?.Quit();
        _driver?.Dispose();
    }

    // [Fact]
    [Fact(Skip = "Need to run separately")]
    public void ReconnectToSessionTest() {
        var conf = new DriverConfig() {
            Reconnect = true,
            Headless = true,
            AutoClose = true,
            BrowserPath = Configuration.DriverConfig.BrowserPath,
            DriverPath = Configuration.DriverConfig.DriverPath,
        };

        var driverProvider1 = new DriverProvider(conf);
        var driver1 = driverProvider1.GetDriver();
        _driver = driver1;
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
        var options = new ChromeOptions() { BinaryLocation = Configuration.DriverConfig.BrowserPath };
        options.AddArgument("headless");
        var myChrome = new ChromeDriver(Configuration.DriverConfig.DriverPath, options);
        new BrowserContent().SetBody($"<h1>{pageTitle}</h1>");
        Configuration.DriverConfig.AutoClose = true;

        //set configuration
        Configuration.SetCustomBrowser(() => myChrome);

        //assert
        ShouldHaveText(S("h1"), pageTitle);

        // close browser
        myChrome.Close();
        myChrome.Dispose();
    }

    // [Fact] public void InvalidBrowserPath() {
    //     Configuration.DriverConfig.BrowserPath = "lol";
    //
    //     var action = () => DriverApi.Driver;
    //     action.Should().Throw<Exception>()
    //         .WithMessage("unknown error: no chrome binary at lol");
    // }
}