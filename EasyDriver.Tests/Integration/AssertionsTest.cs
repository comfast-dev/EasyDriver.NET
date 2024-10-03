using EasyDriver.Tests.Util;
using EasyDriver.Tests.Util.Hooks;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

public class AssertionsTest : IntegrationBase {
    public AssertionsTest(ITestOutputHelper output, AssemblyFixture fix) : base(output, fix) { }

    [Fact] void WaitForFail() {
        ShouldThrow(() => S("html >> lolz").WaitForAppear(100),
            "html >> lolz");
    }
}