using System.Text;
using Comfast.EasyDriver.Core.Events;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration.Events;

public class ActionsEventsTest : IDisposable {
    private readonly StringBuilder _eventLog = new();
    private readonly ITestOutputHelper _output;

    private EventHandler<BeforeEventArgs> _beforeEvent;
    private EventHandler<AfterEventArgs> _afterEvent;

    public ActionsEventsTest(ITestOutputHelper output) {
        _output = output;
        _beforeEvent = (obj, args) => _eventLog.AppendLine("Before: " + args.Name);
        _afterEvent = (obj, args) => _eventLog.AppendLine($"After {args.Name}");
        ActionsEvents.BeforeEvents += _beforeEvent;
        ActionsEvents.AfterEvents += _afterEvent;
    }

    public void Dispose() {
        ActionsEvents.BeforeEvents -= _beforeEvent;
        ActionsEvents.AfterEvents -= _afterEvent;
    }

    [Fact] void BeforeEventTest() {
        var aa = S("body").InnerHtml;

        _output.WriteLine(_eventLog.ToString());
        Assert.Equal(_eventLog.ToString(), @"
Before: InnerHtml
Before: GetAttribute
Before: FindElement
After FindElement
After GetAttribute
After InnerHtml
".TrimStart());



    }
}