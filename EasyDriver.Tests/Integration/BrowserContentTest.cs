using Comfast.EasyDriver.Ui;
using EasyDriver.Tests.Util;
using FluentAssertions;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

public class BrowserContentTest : IntegrationBase {
    public BrowserContentTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    [Fact(Skip = "unstable on chrome")] void TrustedHtmlTest() {
        try {
            var func = () => {
                GetDriver().Url ="chrome://settings/privacy";
                _browserContent.SetBody("<ul><li>xxx</li></ul>");
            };
            func.Should().NotThrow();
        } finally {
            GetDriver().Url = "about:blank";
        }
    }
}