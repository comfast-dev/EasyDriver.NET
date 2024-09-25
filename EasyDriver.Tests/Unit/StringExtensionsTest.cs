using Comfast.Commons.Utils;
using FluentAssertions;

namespace EasyDriver.Tests.Unit;

public class StringExtensionsTest {
    [Fact] void MaxLengthTest() {
        var tenCharString = "1234567890";

        tenCharString.TrimToMaxLength(5).Should().Be("12345...");
        tenCharString.TrimToMaxLength(15).Should().Be(tenCharString);
    }
}