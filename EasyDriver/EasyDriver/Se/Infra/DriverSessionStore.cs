using Comfast.Commons.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Comfast.EasyDriver.Se.Infra;

public static class DriverSessionStore {
    private const string Separator = "#";
    private static readonly TempFile SessionTempFile = new("WebDriverSessionInfo.txt");

    // ReSharper disable once MemberCanBePrivate.Global
    public static void StoreSessionInfo(WebDriver driver) {
        var ssid = driver.ReadField<string>("SessionId.sessionOpaqueKey");
        var uri = driver.ReadField<string>("CommandExecutor.HttpExecutor.remoteServerUri.AbsoluteUri");
        SessionTempFile.Write($"{uri}{Separator}{ssid}");
    }

    /// <summary>
    /// Tries to restore WebDriver session..
    /// It is only possible when:
    /// 1. driver process still running e.g. chromedriver.exe
    /// 2. browser still running
    /// 3. Data is stored - called method <see cref="StoreSessionInfo"/>
    /// </summary>
    /// <param name="newDriverProvider">WebDriver provider in case of restore fail</param>
    /// <returns></returns>
    public static WebDriver RestoreSessionOrElse(Func<WebDriver> newDriverProvider) {
        try {
            var sessionInfo = SessionTempFile.Read().Split(Separator);
            var recreatedDriver = new RemoteWebDriver(
                new FixedSessionExecutor(sessionInfo[0], sessionInfo[1]),
                new RemoteSessionSettings());

            recreatedDriver.FindElements(By.CssSelector("html"));
            return recreatedDriver;
        } catch (Exception) {
            var newDriver = newDriverProvider.Invoke();
            StoreSessionInfo(newDriver);
            return newDriver;
        }
    }
}