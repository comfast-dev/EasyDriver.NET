using System.Text;
using Comfast.EasyDriver.Core.Events;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration.Events;

public class WebDriverEventsTest : IDisposable {
    private readonly StringBuilder _eventLog = new();
    private readonly ITestOutputHelper _output;

    private EventHandler<BeforeEventArgs> _beforeWd;
    private EventHandler<AfterEventArgs> _afterWd;

    public WebDriverEventsTest(ITestOutputHelper output) {
        _output = output;
        _beforeWd = (obj, args) => _eventLog.AppendLine($"Before: {args.Name}");
        _afterWd = (obj, args) => _eventLog.AppendLine($"After: {args.Name}");
        WebDriverEvents.BeforeEvents += _beforeWd;
        WebDriverEvents.AfterEvents += _afterWd;
    }

    public void Dispose() {
        WebDriverEvents.BeforeEvents -= _beforeWd;
        WebDriverEvents.AfterEvents -= _afterWd;
    }

    [Fact(Skip = "fails on parallel run")] void MatchEventsTest() {
        var aa = S("body").InnerHtml;

        _output.WriteLine(_eventLog.ToString());
        Assert.EndsWith(@"
Before: findElement
After: findElement
Before: executeScript
After: executeScript
".TrimStart(), _eventLog.ToString());
    }
}