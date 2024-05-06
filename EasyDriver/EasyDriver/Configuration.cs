using Comfast.Commons.Utils;
using Comfast.EasyDriver.Se.Infra;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;

public class Configuration {
    public static DriverConfig DriverConfig { get; set; } = ConfigLoader.Load<DriverConfig>("AppConfig.json", "DriverConfig");
    
    private static Func<IWebDriver> _driverProvider = () => DriverSource.Driver;
    
    public static IWebDriver GetDriver() => _driverProvider.Invoke();
    
    public static void SetCustomDriver(IWebDriver driver) {
        _driverProvider = () => driver;
    }
    
    public static void SetCustomDriver(Func<IWebDriver> provider) {
        _driverProvider = provider;
    }
}