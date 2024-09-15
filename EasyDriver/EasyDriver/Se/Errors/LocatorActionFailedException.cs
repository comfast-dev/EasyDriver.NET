using System.Text.RegularExpressions;
using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;

namespace Comfast.EasyDriver.Se.Errors;

public class LocatorActionFailedException : LocatorException {
    public string ActionName { get; }
    public string? ElementHtml { get; set; }

    public LocatorActionFailedException(ILocator locator, string actionName, Exception? cause = null)
        : base("Locator action failed", locator, cause) {
        ActionName = actionName;
    }

    public override string Message =>
        ClearNewLines(@$"
Action '{ActionName}' failed at element: '{Locator.Description}'
Locator: {Locator.Selector}
{OptionalLine("Element HTML", ElementHtml?.TrimToMaxLength(300))}

{OptionalLine("Screenshot", ScreenshotPath)}
{OptionalLine("Snapshot", SnapshotPath)}
{OptionalLine("Cause", CleanSeleniumCauseMessage(InnerException))}
");
}