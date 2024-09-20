using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Finder;

namespace Comfast.EasyDriver.Se.Errors;

public class LocatorNotFoundException : LocatorException {
    public int FailedSelectorIndex { get; }

    public LocatorNotFoundException(ILocator locator, int failedSelectorIndex, Exception? cause = null) : base(
        "Locator not found", locator, cause) {
        FailedSelectorIndex = failedSelectorIndex;
    }

    public override string Message =>
        ClearNewLines(@$"
Not found element: '{Locator.Description}'
Locator: {Locator.CssOrXpath}
at:      {CreateSpacesOffsetToFailedSelector()}^
{OptionalLine("Screenshot", ScreenshotPath)}
{OptionalLine("Snapshot", SnapshotPath)}
{OptionalLine("Cause", CleanSeleniumCauseMessage(InnerException))}
");

    private string CreateSpacesOffsetToFailedSelector() {
        int offset = new SelectorChain(Locator.CssOrXpath).SelectorsArray.Take(FailedSelectorIndex)
            .Aggregate(0, (acc, x) => acc + x.Length + SelectorChain.SelectorSeparator.Length);
        return new string(' ', offset);
    }
}