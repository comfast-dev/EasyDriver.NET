using Comfast.EasyDriver.Locator;
using Xunit;

namespace EasyDriver.Test.Integration;

public class XpathTest {
    [Fact] public void EscapeTextTest() {
        test("It's \"hard\" text 'to' match", "concat('It', \"'\", 's \"hard\" text ', \"'\", 'to', \"'\", ' match')");
        test("Some text", "'Some text'");
        test("I'am", "\"I'am\"");
    }

    private void test(string inputText, string expectedEscapedXpathText) {
        Assert.Equal(expectedEscapedXpathText, inputText.EscapeXpath());
    }
}