using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Errors;
using EasyDriver.Tests.Util;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Unit;

public class ExceptionsTest : UnitBase {
    public ExceptionsTest(ITestOutputHelper output) : base(output) { }

    [Fact] public void ElementNotFoundExceptionTest() {
        ILocator table = S("//html/table", "My Table");
        ILocator activeCell = table._S("td.active span", "Active cell");

        var exception = new LocatorNotFoundException(activeCell, 1);
        exception.ScreenshotPath = "c:/some/path.png";
        exception.SnapshotPath = "c:/some/path.html";

        Assert.Equal(exception.Message, @"
Not found element: 'Active cell'
Locator: //html/table >> td.active span
at:                      ^
Screenshot: c:/some/path.png
Snapshot: c:/some/path.html".Trim());
    }

    [Fact] public void ActionFailedExceptionTest() {
        ILocator table = S("//html/table", "My Table");
        ILocator activeCell = table._S("td.active span", "Active cell");

        var exception = new LocatorActionFailedException(activeCell, "Click");

        exception.ElementHtml = "<div class=\"some class\">content</div>";
        exception.ScreenshotPath = "c:/some/path.png";
        exception.SnapshotPath = "c:/some/path.html";

        Assert.Equal(exception.Message, @"
Action 'Click' failed at element: 'Active cell'
Locator: //html/table >> td.active span
Element HTML: <div class=""some class"">content</div>
Screenshot: c:/some/path.png
Snapshot: c:/some/path.html".Trim());
    }

    [Fact] public void ClearNewLines() {
        var ex = new LocatorActionFailedException(S("//html"), "Click", new Exception("example"));
        Assert.Equal(ex.Message, @"
Action 'Click' failed at element: 'Locator'
Locator: //html
Cause: example".Trim());
    }
}