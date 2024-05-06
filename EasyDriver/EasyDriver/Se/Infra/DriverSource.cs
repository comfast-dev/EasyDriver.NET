using Comfast.Commons.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Comfast.EasyDriver.Se.Infra;

/**
 * Return one Driver instance per thread
 */
public class DriverSource {
    private static readonly ThreadLocal<WebDriver> Instances = new(ProvideDriverInstance, true);
    public static WebDriver Driver => GetDriver();

    /**
     * Provide WebDriver Instance.
     * One per thread.
     * Can be called multiple times.
     */
    public static WebDriver GetDriver() {
        if (Configuration.DriverConfig.Reconnect 
            && Instances.Values.Count > 1
            ) {
            throw new Exception(@"
Reconnect feature isn't compatible with parallel runs. Possible solutions:
- Set DriverConfig.Reconnect flag to false in AppConfig.json
- Change runner configuration to run tests in one thread
");
        }
        
        return Instances.Value ?? throw new Exception("never happen");
    }

    private static WebDriver ProvideDriverInstance() {
        bool reconnect = Configuration.DriverConfig.Reconnect;
        bool autoClose = Configuration.DriverConfig.AutoClose;

        var driver = reconnect && !autoClose
            ? DriverSessionStore.RestoreSessionOrElse(RunNewDriver)
            : RunNewDriver();

        if (autoClose) AddShutdownHook(driver);

        return driver;
    }

    /**
     * Make sure WebDriver is closed after end of process
     */
    private static void AddShutdownHook(WebDriver driver) {
        AppDomain.CurrentDomain.ProcessExit += (s, e) => {
            driver.Close(); // closes browser
            driver.Dispose(); // closes driver process ( e.g. chromedriver.exe ) 
        };
    }

    /**
     * Run new WebDriver instance
     */
    private static ChromeDriver RunNewDriver() {
        ChromeOptions options = new();
        if(Configuration.DriverConfig.Headless) options.AddArguments("headless");
        options.BinaryLocation = Configuration.DriverConfig.BrowserPath;
        
        return new ChromeDriver(Configuration.DriverConfig.DriverPath, options);
    }
}