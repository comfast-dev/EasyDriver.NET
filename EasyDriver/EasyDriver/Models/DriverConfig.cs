namespace Comfast.EasyDriver.Models;

/// <summary>
/// All information required used to run the browser
/// </summary>
public class DriverConfig {
    /// <summary>
    /// Used to differentiate which driver to run.
    /// e.g. "chrome", "firefox", "edge"
    /// </summary>
    public string BrowserName { get; set; } = "chrome";

    /// <summary>
    /// e.g. c:/browsers/chrome120/chrome.exe
    /// </summary>
    public string BrowserPath { get; set; }

    /// <summary>
    /// Path to chromedriver.exe / firefoxdriver.exe / ...
    /// e.g. c:/drivers/chromedriver120
    /// </summary>
    public string DriverPath { get; set; }

    /// <summary>
    /// If true DriverProvider will try to reconnect to running browser
    /// </summary>
    public bool Reconnect { get; set; }

    /// <summary>
    /// If true - browser will be automatically closed after end of process
    /// </summary>
    public bool AutoClose { get; set; }

    /// <summary>
    /// If true - browser will run without UI
    /// </summary>
    public bool Headless { get; set; } = false;

    /// <summary>
    /// Where browser should download files
    /// </summary>
    public string DownloadPath { get; set; } = Path.Combine(Path.GetTempPath(), "EasyDriver", "downloads");
}