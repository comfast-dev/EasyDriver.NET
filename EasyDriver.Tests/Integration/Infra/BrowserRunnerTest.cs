﻿using Comfast.Commons.Utils;
using Comfast.EasyDriver.Core.Finder;
using Comfast.EasyDriver.Core.Infra.Browser;
using EasyDriver.Tests.Util;
using EasyDriver.Tests.Util.Hooks;
using FluentAssertions;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration.Infra;

public class BrowserRunnerTest : IntegrationBase, IDisposable {
    private IWebDriver? _currentDriver;
    public BrowserRunnerTest(ITestOutputHelper output, AssemblyFixture fix) : base(output, fix) { }

    [Fact] void WindowSizeTest() {
        //given
        var conf = Configuration.BrowserConfig.Copy();
        int width = 888;
        int height = 444;
        conf.WindowSize = $"{width}x{height}";
        _currentDriver = new BrowserRunner(conf).RunNewBrowser();

        //expect
        var actualSize = _currentDriver.Manage().Window.Size;
        ShouldEqual(actualSize.Height, height);
        ShouldEqual(actualSize.Width, width);
    }

    [Fact] void WindowMaximizedTest() {
        //given
        var conf = Configuration.BrowserConfig.Copy();
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
        var conf = Configuration.BrowserConfig.Copy();
        var runner = new BrowserRunner(conf);

        conf.WindowSize = "xd";
        ShouldThrow(() => runner.RunNewBrowser(),
            "Invalid WindowSize='xd', accepted are: 1234x567 | default | maximized");

        conf.WindowSize = "123lol456";
        ShouldThrow(() => runner.RunNewBrowser(), $"Invalid WindowSize='{conf.WindowSize}'");

        conf.WindowSize = "";
        ShouldThrow(() => runner.RunNewBrowser(), $"Invalid WindowSize='{conf.WindowSize}'");

        conf.WindowSize = null!;
        ShouldThrow(() => runner.RunNewBrowser(), $"Invalid WindowSize='{conf.WindowSize}'");
    }

    // [Fact]
    [Fact(Skip = "Unstable, dependent on external proxy provider")]
    void RunWithProxy() {
        // find some proxy here https://free-proxy-list.net/
        Uri proxyUrl = new Uri("http://155.94.241.134:3128");

        var conf = Configuration.BrowserConfig.Copy();
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