using Comfast.EasyDriver.Lib;
using OpenQA.Selenium;
using Xunit.Abstractions;
using Xunit.Extensions.AssemblyFixture;

[assembly: TestFramework(AssemblyFixtureFramework.TypeName, AssemblyFixtureFramework.AssemblyName)]

namespace EasyDriver.Tests.Util;

public class IntegrationBase : IAssemblyFixture<IntegrationFixture> {
    protected readonly ITestOutputHelper _output;
    private readonly IntegrationFixture _fix;

    protected readonly IWebDriver _driver;
    protected readonly BrowserContent _browserContent;

    public IntegrationBase(ITestOutputHelper output, IntegrationFixture fix) {
        _output = output;
        _fix = fix;
        _driver = GetDriver();
        _browserContent = new(_driver);
    }
}

public class IntegrationFixture : IDisposable {
    //Before all hook
    public IntegrationFixture() { }
    //After all hook

    public void Dispose() {
        if (!Configuration.BrowserConfig.Reconnect) {
            DriverProvider.CloseAllDrivers();
        }
    }
}