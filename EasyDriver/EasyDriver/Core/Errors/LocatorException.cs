using System.Text.RegularExpressions;
using Comfast.EasyDriver.Models;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Core.Errors;

public abstract class LocatorException : Exception {
    protected ILocator Locator { get; }
    public string? ScreenshotPath { get; set; }
    public string? SnapshotPath { get; set; }

    protected LocatorException(string baseMessage, ILocator locator, Exception? cause = null)
        : base(baseMessage, cause) {
        Locator = locator;
    }

    protected string ClearNewLines(string errorMessage) {
        return Regex.Replace(errorMessage, "(\r\n|\r|\n)+", "\r\n").Trim();
    }

    protected string OptionalLine(string lineLabel, string? content) {
        return content == null ? "" : $"{lineLabel}: {content}";
    }

    protected string? CleanSeleniumCauseMessage(Exception? cause) {
        if (cause is WebDriverException) {
            return Regex.Replace(cause.Message, @"\s*\(Session info[\s\S]+$", "");
        }

        return cause?.Message;
    }
}