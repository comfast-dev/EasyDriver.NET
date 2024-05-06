using Comfast.Commons.Utils;
using Comfast.EasyDriver.Se.Infra;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

public static class Configuration {
    private static Func<IWebDriver> _driverProvider = () => DriverProvider.Driver;

    public static DriverConfig DriverConfig { get; set; } =
        ConfigLoader.Load<DriverConfig>("AppConfig.json", "DriverConfig");

    public static LocatorConfig LocatorConfig { get; set; } =
        ConfigLoader.Load<LocatorConfig>("AppConfig.json", "LocatorConfig");

    public static IWebDriver GetDriver() => _driverProvider.Invoke();

    public static void SetCustomDriver(IWebDriver driver) {
        _driverProvider = () => driver;
    }

    public static void SetCustomDriver(Func<IWebDriver> provider) {
        _driverProvider = provider;
    }
}