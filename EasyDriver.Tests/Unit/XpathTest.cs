using Comfast.EasyDriver.Se.Finder;
using JetBrains.Annotations;

namespace EasyDriver.Tests.Unit;

[TestSubject(typeof(Xpath))]
public class XpathTest {
    [Fact] public void EscapeTextTest() {
        DoTest("It's \"hard\" text 'to' match",
            "concat('It', \"'\", 's \"hard\" text ', \"'\", 'to', \"'\", ' match')");
        DoTest("Some text", "'Some text'");
        DoTest("I'am", "\"I'am\"");
    }

    private void DoTest(string inputText, string expectedEscapedXpathText) {
        Assert.Equal(expectedEscapedXpathText, inputText.EscapeQuotesInXpath());
    }
}