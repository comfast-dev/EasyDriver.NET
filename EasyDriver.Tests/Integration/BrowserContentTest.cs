using Comfast.EasyDriver;
using Comfast.EasyDriver.Ui;
using FluentAssertions;

namespace EasyDriver.Tests.Integration;

public class BrowserContentTest {
    [Fact(Skip = "unstable on chrome")] void TrustedHtmlTest() {
        try {
            var func = () => {
                DriverApi.NavigateTo("chrome://settings/privacy");
                new BrowserContent().SetBody("<ul><li>xxx</li></ul>");
            };
            func.Should().NotThrow();
        } finally {
            DriverApi.NavigateTo("about:blank");
        }
    }
}