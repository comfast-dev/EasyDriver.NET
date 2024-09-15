using Comfast.Commons.Utils;
using Comfast.EasyDriver.Se.Finder;
using Comfast.EasyDriver.Se.Infra.Browser;
using EasyDriver.Tests.Util;
using FluentAssertions;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration.Infra;

public class BrowserRunnerTest : IntegrationBase, IDisposable {
    private IWebDriver? _currentDriver;
    public BrowserRunnerTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    [Fact] void ScreenSizeTest() {
        //given
        var conf = EasyDriverConfig.BrowserConfig.Copy();
        int width = 888;
        int height = 444;
        conf.WindowSize = $"{width}x{height}";
        _currentDriver = new BrowserRunner(conf).RunNewBrowser();

        //expect
        var actualSize = _currentDriver.Manage().Window.Size;
        ShouldEqual(actualSize.Height, height);
        ShouldEqual(actualSize.Width, width);
    }

    [Fact] void ScreenMaximizedTest() {
        //given
        var conf = EasyDriverConfig.BrowserConfig.Copy();
        conf.WindowSize = "maximized";
        _currentDriver = new BrowserRunner(conf).RunNewBrowser();

        //expect
        var actualSize = _currentDriver.Manage().Window.Size;
        _currentDriver.Manage().Window.Maximize();
        var maximizedSize = _currentDriver.Manage().Window.Size;

        ShouldEqual(actualSize, maximizedSize);
    }

    [Fact] void ScreenSizeInvalidTest() {
        //given
        var conf = EasyDriverConfig.BrowserConfig.Copy();
        var runner = new BrowserRunner(conf);

        conf.WindowSize = "xd";
        ShouldThrow(() => runner.RunNewBrowser(),
            "Invalid ScreenSize='xd', accept 1234x567 | default | maximized");

        conf.WindowSize = "123,456";
        ShouldThrow(() => runner.RunNewBrowser(), $"Invalid ScreenSize='{conf.WindowSize}'");

        conf.WindowSize = "";
        ShouldThrow(() => runner.RunNewBrowser(), $"Invalid ScreenSize='{conf.WindowSize}'");

        conf.WindowSize = null!;
        ShouldThrow(() => runner.RunNewBrowser(), $"Invalid ScreenSize='{conf.WindowSize}'");
    }

    // [Fact]
    [Fact(Skip = "Unstable, dependent on external proxy provider")]
    void RunWithProxy() {
        // find some proxy here https://free-proxy-list.net/
        Uri proxyUrl = new Uri("http://155.94.241.134:3128");

        var conf = EasyDriverConfig.BrowserConfig;
        conf.Headless = false;
        conf.ProxyUrl = proxyUrl.OriginalString;

        _currentDriver = new BrowserRunner(conf).RunNewBrowser();
        _currentDriver.Url = "http://ident.me";

        _currentDriver.Find("body").Text.Should().Be(proxyUrl.Host);
    }

    public void Dispose() {
        _currentDriver?.Dispose();
    }
}