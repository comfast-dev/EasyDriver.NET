using EasyDriver.Tests.Util;
using EasyDriver.Tests.Util.Hooks;
using FluentAssertions;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

public class BrowserContentTest : IntegrationBase {
    public BrowserContentTest(ITestOutputHelper output, AssemblyFixture fix) : base(output, fix) { }

    [Fact(Skip = "unstable on chrome")] void TrustedHtmlTest() {
        try {
            var func = () => {
                GetDriver().Url = "chrome://settings/privacy";
                _browserContent.SetBody("<ul><li>xxx</li></ul>");
            };
            func.Should().NotThrow();
        } finally {
            GetDriver().Url = "about:blank";
        }
    }
}