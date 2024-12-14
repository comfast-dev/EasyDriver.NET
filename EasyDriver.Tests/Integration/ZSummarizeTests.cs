using EasyDriver.Tests.Util;
using EasyDriver.Tests.Util.Hooks;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

/// <summary> Run at the end. </summary>
public class ZSummarizeTests : IntegrationBase {
    public ZSummarizeTests(ITestOutputHelper output, AssemblyFixture fix) : base(output, fix) { }

    [Fact] void PrintSummaryStats() {
        var d = GetDriver();
        S("html").HasCssClass("abc");

        Thread.Sleep(5000);
        _output.WriteLine(ActionsEvents.Stats?.PrintStats());
        _output.WriteLine(WebDriverEvents.Stats?.PrintStats());
    }
}