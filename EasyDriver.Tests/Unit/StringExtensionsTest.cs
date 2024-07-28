using Comfast.Commons.Utils;
using FluentAssertions;

namespace EasyDriver.Tests.Unit;

public class StringExtensionsTest {
    [Fact] void MaxLengthTest() {
        var tenCharString = "1234567890";

        tenCharString.MaxLength(5).Should().Be("12345");
        tenCharString.MaxLength(15).Should().Be(tenCharString);
    }

    [Fact] void RgxReplaceTest() {
        var test = "one one two two";
        test.RgxReplace("two", "lol").Should().Be("one one lol lol");
        test.RgxReplace("xd", "").Should().Be(test);
    }


    [Fact] void RgxMatchTest() {
        var test = "one one two two";
        test.RgxMatch("(one) two").Should().Be("one two");
        test.RgxMatch("(one) (two)", 1).Should().Be("one");
        test.RgxMatch("(one) (two)", 2).Should().Be("two");
        test.RgxMatch("xd").Should().BeNull();
        test.RgxMatch("xd", 2).Should().BeNull();
    }
    // [Fact] void Aaa() {}
    // [Fact] void Aaa() {}
    // [Fact] void Aaa() {}
}