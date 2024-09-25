using Comfast.Commons.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Comfast.EasyDriver.Core.Infra;

/// <summary> Stores WebDriver session in temp file and restores it in new process.</summary>
public class WebDriverSessionStore {
    private const string Separator = "#";
    private readonly string _tempFilePath = Path.Combine(Path.GetTempPath(), "EasyDriver/WebDriverSessionInfo.txt");

    public WebDriverSessionStore() {
        var dir = Path.GetDirectoryName(_tempFilePath) ?? throw new("Invalid file directory: " + _tempFilePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        if (!File.Exists(_tempFilePath)) File.Create(_tempFilePath);
    }

    /// <summary> Extract data: SessionId, DriverUrl needed to connect to the browser in temp file.</summary>
    /// <param name="driver"></param>
    public void StoreSessionInfo(IWebDriver driver) {
        var ssid = driver.ReadField<string>("SessionId.sessionOpaqueKey");
        var uri = driver.ReadField<string>("CommandExecutor.HttpExecutor.remoteServerUri.AbsoluteUri");
         File.WriteAllText(_tempFilePath, $"{uri}{Separator}{ssid}");
    }

    /// <summary>
    /// Tries to restore WebDriver session.
    /// It is only possible when:
    /// 1. driver process still running e.g. chromedriver.exe
    /// 2. browser still running
    /// 3. Data is stored - method <see cref="StoreSessionInfo"/> called in previous run
    /// </summary>
    /// <param name="newDriverProvider">WebDriver provider in case of restore fail</param>
    /// <returns></returns>
    public IWebDriver RestoreSessionOrRunNewDriver(Func<IWebDriver> newDriverProvider) {
        try {
            var sessionInfo = File.ReadAllText(_tempFilePath).Split(Separator);
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