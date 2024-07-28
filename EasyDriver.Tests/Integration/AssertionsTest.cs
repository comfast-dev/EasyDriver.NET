using EasyDriver.Tests.Util;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

public class AssertionsTest : IntegrationBase {
    public AssertionsTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    [Fact] void WaitForFail() {
        ShouldThrow(() => S("html >> lolz").WaitForAppear(100),
            "html >> lolz");
    }
}