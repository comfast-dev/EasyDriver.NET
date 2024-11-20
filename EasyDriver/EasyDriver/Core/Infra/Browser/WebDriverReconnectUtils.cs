using Comfast.Commons.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Comfast.EasyDriver.Core.Infra.Browser;

public static class WebDriverReconnectUtils {
    private const string Separator = "#";

    /// <summary> Extract data: SessionId, DriverUrl needed to connect to the browser.</summary>
    public static string ExtractSessionString(IWebDriver driver)
        => $"{ExtractUri(driver)}{Separator}{ExtractSsid(driver)}";

    public static (string url, string ssid) ParseSessionString(string sessionString) {
        var data = sessionString.Split(Separator);
        return (data[0], data[1]);
    }

    public static RemoteWebDriver CreateDriver(string sessionString) {
        var (url, ssid) = ParseSessionString(sessionString);
        return new(
            new FixedSessionExecutor(url, ssid),
            new RemoteSessionSettings());
    }

    public static bool TestConnection(this IWebDriver driver) {
        try {
            var shouldNotThrow = driver.Title;
            return true;
        } catch (Exception) {
            return false;
        }
    }
    //new WdClient(sessionString).Title.IsSuccessStatusCode

    private static string ExtractUri(IWebDriver driver) {
        try {
            return driver.ReadField<string>("CommandExecutor.HttpExecutor.remoteServerUri.AbsoluteUri")!;
        } catch (ArgumentException) {
            return driver.ReadField<string>("CommandExecutor.remoteServerUri.AbsoluteUri")!;
        }
    }

    private static string ExtractSsid(IWebDriver driver) =>
        driver.ReadField<string>("SessionId.sessionOpaqueKey")!;
}