using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra;
using Comfast.EasyDriver.Ui;
using FluentAssertions;
using OpenQA.Selenium.Chrome;
using Configuration = Comfast.EasyDriver.Configuration;

namespace EasyDriver.Tests.Integration;

public class DriverProviderTest : IDisposable {
    private readonly string _driverPath;
    private readonly string _browserPath;

    public DriverProviderTest() {
        _driverPath = Configuration.DriverConfig.DriverPath;
        _browserPath = Configuration.DriverConfig.BrowserPath;
    }

    public void Dispose() {
        Configuration.DriverConfig.DriverPath = _driverPath;
        Configuration.DriverConfig.BrowserPath = _browserPath;
    }

    [Fact] public void ReconnectToSessionTest() {
        var conf = new DriverConfig() {
            Reconnect = true,
            Headless = true,
            BrowserPath = Configuration.DriverConfig.BrowserPath,
            DriverPath = Configuration.DriverConfig.DriverPath,
        };

        var driverProvider1 = new DriverProvider(conf);
        var driver1 = driverProvider1.GetDriver();
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