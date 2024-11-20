using Comfast.EasyDriver.Lib;
using EasyDriver.Tests.Util.Hooks;
using OpenQA.Selenium;
using Xunit.Abstractions;
using Xunit.Extensions.AssemblyFixture;

[assembly: TestFramework(AssemblyFixtureFramework.TypeName, AssemblyFixtureFramework.AssemblyName)]

namespace EasyDriver.Tests.Util;

public class IntegrationBase : IAssemblyFixture<AssemblyFixture> {
    protected readonly ITestOutputHelper _output;
    protected readonly AssemblyFixture _fix;
    protected readonly IWebDriver _driver;
    protected readonly BrowserContent _browserContent;

    public IntegrationBase(ITestOutputHelper output, AssemblyFixture fix) {
        _output = output;
        _fix = fix;
        _driver = GetDriver();
        _browserContent = new(_driver);
    }
}