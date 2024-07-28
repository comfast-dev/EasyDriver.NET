using FluentAssertions;

namespace EasyDriver.Tests.Integration;

public class AssertionsTest {
    [Fact] void WaitForFail() {
        ShouldThrow(() => S("html >> lolz").WaitFor(100),
            "html >> lolz");
    }
}