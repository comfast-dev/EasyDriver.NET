using Comfast.EasyDriver;
using FluentAssertions;
using Xunit.Sdk;
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

    // [Fact] public void InvalidDriverPath() {
    //     Configuration.DriverConfig.DriverPath = "C:\\lol\\chrome.txt";
    //
    //     var action = () => DriverApi.Driver;
    //     action.Should().Throw<Exception>()
    //         .WithMessage("Invalid driver path: C:\\lol\\chrome.txt");
    // }
    //
    // [Fact] public void InvalidBrowserPath() {
    //     Configuration.DriverConfig.BrowserPath = "lol";
    //
    //     var action = () => DriverApi.Driver;
    //     action.Should().Throw<Exception>()
    //         .WithMessage("unknown error: no chrome binary at lol");
    // }
}