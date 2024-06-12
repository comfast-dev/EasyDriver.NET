using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Infra;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

public static class Configuration {
    public static DriverConfig DriverConfig { get; set; } =
        ConfigLoader.Load<DriverConfig>("AppConfig.json", "DriverConfig");

    public static LocatorConfig LocatorConfig { get; set; } =
        ConfigLoader.Load<LocatorConfig>("AppConfig.json", "LocatorConfig");


    public static IWebDriver GetDriver() => _driverProvider.GetDriver();

    private static IDriverProvider _driverProvider = new DriverProvider(DriverConfig);
}