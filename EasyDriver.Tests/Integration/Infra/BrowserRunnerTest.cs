using Comfast.EasyDriver.Se.Finder;
using Comfast.EasyDriver.Se.Infra.Browser;
using EasyDriver.Tests.Util;
using FluentAssertions;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration.Infra;

public class BrowserRunnerTest : IntegrationBase {
    public BrowserRunnerTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    // [Fact]
    [Fact(Skip = "Unstable, dependent on external proxy provider")]
    void RunWithProxy() {
        // find some proxy here https://free-proxy-list.net/
        Uri proxyUrl = new Uri("http://155.94.241.134:3128");

        var conf = Configuration.DriverConfig;
        conf.Headless = false;
        conf.ProxyUrl = proxyUrl.OriginalString;

        var driver = new BrowserRunner(conf).RunNewBrowser();
        driver.Url = "http://ident.me";

        driver.Find("body").Text.Should().Be(proxyUrl.Host);
    }
}