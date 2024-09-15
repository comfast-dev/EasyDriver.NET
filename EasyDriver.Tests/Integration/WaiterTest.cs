using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se;
using Comfast.EasyDriver.Se.Finder;
using Comfast.EasyDriver.Ui;
using EasyDriver.Tests.Util;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

public class WaiterTest : IntegrationBase {
    public WaiterTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) {
        _browserContent.OpenResourceFile("test.html");
        EasyDriverConfig.RuntimeConfig.TimeoutMs = LongTime;
    }

    private const int SuccessTime = 100; // for success waits
    private const int ShortTime = 50; // for failed waits
    private const int LongTime = 3000;
    private const int TimeMargin = 200;

    private const int SuccessTimeMax = SuccessTime + TimeMargin;
    private const int ShortTimeMax = ShortTime + TimeMargin;

    private const string H5Html = "<h5>Hello</h5>";
    private const string H5DisplayNone = "<h5 style='display:none'>Hello</h5>";

    private readonly ILocator _h3 = S("#spawn .target h3");
    private readonly ILocator _h4 = S("#spawn .target h4");
    private readonly ILocator _h5 = S("#spawn .target h5");
    private readonly ILocator _p = S("#spawn .target p");

    [Fact] void WaitForTest() {
        ScheduleReloadHtmlAfter(H5DisplayNone, SuccessTime);
        ShouldEndInTime(() => _h5.WaitFor(), SuccessTime, SuccessTimeMax);

        ShouldThrowInTime(() => _h3.WaitFor(ShortTime), ShortTime, ShortTime + TimeMargin,
            "Last error: 'Not found element:");
    }

    [Fact] void WaitForAppearTest() {
        ScheduleReloadHtmlAfter(H5Html, SuccessTime);
        ShouldEndInTime(() => _h5.WaitForAppear(), SuccessTime, SuccessTimeMax);

        ReloadHtml(H5DisplayNone);
        ShouldThrowInTime(() => _h5.WaitForAppear(ShortTime), ShortTime, ShortTimeMax,
            $"Wait failed after {ShortTime}ms for action: Element appear");
        ShouldThrowInTime(() => _h3.WaitForAppear(ShortTime), ShortTime, ShortTimeMax,
            $"Wait failed after {ShortTime}ms for action: Element appear");
    }

    [Fact] void WaitForDisappearTest() {
        ReloadHtml(H5Html);
        ScheduleReloadHtmlAfter("<p>Nothing to do here</p>", SuccessTime);

        ShouldEndInTime(() => _h5.WaitForDisappear(), SuccessTime, SuccessTimeMax);

        ShouldThrowInTime(() => _p.WaitForDisappear(ShortTime), ShortTime, ShortTimeMax,
            $"Wait failed after {ShortTime}ms for action: Element disappear:");
    }

    [Fact] void WaitForAnyTest() {
        ScheduleReloadHtmlAfter(H5Html, SuccessTime);

        ShouldEndInTime(() => Waiter.WaitForAny(_h3, _h4, _h5), SuccessTime, SuccessTimeMax);
        ShouldEqual(Waiter.WaitForAny(_h3, _h4, _h5), 2);

        ShouldThrowInTime(() => Waiter.WaitForAny(ShortTime, _h3, _h4), ShortTime, ShortTimeMax,
            $"Wait failed after {ShortTime}ms for action: Any element of these:");
    }

    [Fact] void WaitForReloadTest() {
        ReloadHtml(H5Html);

        var reloadAfterTimeAction = () => ScheduleReloadHtmlAfter(H5Html, SuccessTime);
        ShouldEndInTime(() => _h5.WaitForReload(reloadAfterTimeAction), SuccessTime, SuccessTimeMax);

        var dontReloadAction = () => { };
        ShouldThrowInTime(() => _h5.WaitForReload(dontReloadAction, ShortTime), ShortTime, ShortTimeMax,
            $"Wait failed after {ShortTime}ms for action: Reload element:");
    }

    [Fact] void WaitForStablePageTest() {
        S("#redirect input[name=interval]").SetValue("100");
        S("#redirect input[name=count]").SetValue("2");
        S("#redirect button").Click();
        //page unstable for 200ms(redirects)+300ms(wait time)
        ShouldEndInTime(() => Waiter.WaitForStablePage(300), 500, 2000);

        //page is stable
        var stableTime = 100;
        ShouldEndInTime(() => Waiter.WaitForStablePage(stableTime), stableTime, stableTime + TimeMargin);

        S("#redirect button").Click();
        ShouldThrowInTime(() => Waiter.WaitForStablePage(500, ShortTime), ShortTime, ShortTimeMax,
            $"Page is unstable, reloading every <500ms. Timed out after {ShortTime}ms.");
    }

    private void ReloadHtml(string html) {
        _driver.ExecuteJs<string>("document.querySelector('#spawn .target').innerHTML = arguments[0]",
            html);
    }

    private void ScheduleReloadHtmlAfter(string html, int timeoutMs) {
        _driver.ExecuteJs<string>(
            "setTimeout(() => document.querySelector('#spawn .target').innerHTML = arguments[0], arguments[1])",
            html, timeoutMs);
    }
}