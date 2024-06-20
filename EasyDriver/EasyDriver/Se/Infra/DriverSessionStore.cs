using Comfast.Commons.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Comfast.EasyDriver.Se.Infra;

/// <summary>
/// Stores WebDriver session in temp file and restores it in new process.
/// </summary>
public class DriverSessionStore {
    private const string Separator = "#";
    private readonly TempFile _sessionTempFile = new("WebDriverSessionInfo.txt");

    // ReSharper disable once MemberCanBePrivate.Global
    /// <summary>
    /// 1. Extract data needed to connect to the browser: SessionId and driver Url
    /// 2. Store it in temp file.
    /// </summary>
    /// <param name="driver"></param>
    public void StoreSessionInfo(IWebDriver driver) {
        var ssid = driver.ReadField<string>("SessionId.sessionOpaqueKey");
        var uri = driver.ReadField<string>("CommandExecutor.HttpExecutor.remoteServerUri.AbsoluteUri");
        _sessionTempFile.Write($"{uri}{Separator}{ssid}");
    }

    /// <summary>
    /// Tries to restore WebDriver session..
    /// It is only possible when:
    /// 1. driver process still running e.g. chromedriver.exe
    /// 2. browser still running
    /// 3. Data is stored - method <see cref="StoreSessionInfo"/> called in previous run
    /// </summary>
    /// <param name="newDriverProvider">WebDriver provider in case of restore fail</param>
    /// <returns></returns>
    public IWebDriver RestoreSessionOrElse(Func<IWebDriver> newDriverProvider) {
        try {
            var sessionInfo = _sessionTempFile.Read().Split(Separator);
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