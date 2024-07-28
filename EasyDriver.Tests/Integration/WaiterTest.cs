using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se;
using EasyDriver.Tests.Integration.Infra;
using EasyDriver.Tests.Util;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

public class WaiterTest : IntegrationBase {
    public WaiterTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    private const int Time = 100; // for success waits
    private const int ShortTime = 50; // for failed waits
    private const int LongTime = 3000;
    private const int TimeMargin = 200;

    private readonly Waiter _longWaiter = new(LongTime); // for success waits
    private readonly Waiter _shortWaiter = new(ShortTime, 10); // for failed waits

    private const string H5Html = "<h5>Hello</h5>";
    private const string H5DisplayNone = "<h5 style='display:none'>Hello</h5>";

    private readonly ILocator _h3 = S("#spawn .target h3");
    private readonly ILocator _h4 = S("#spawn .target h4");
    private readonly ILocator _h5 = S("#spawn .target h5");
    private readonly ILocator _p = S("#spawn .target p");

    [Fact] void WaitForTest() {
        SpawnHtmlAfterMs(H5DisplayNone, Time);
        ShouldEndInTime(() => _h5.WaitFor(), Time, Time + TimeMargin);

        ShouldThrowInTime(() => _h3.WaitFor(ShortTime), ShortTime, ShortTime + TimeMargin,
            "Element find fail:\n#spawn .target h3");
    }

    [Fact] void WaitForAppearTest() {
        SpawnHtmlAfterMs(H5Html, Time);
        ShouldEndInTime(() => _h5.WaitForAppear(), Time, Time + TimeMargin);

        SpawnHtml(H5DisplayNone);
        ShouldThrowInTime(() => _h5.WaitForAppear(ShortTime), ShortTime, ShortTime + TimeMargin,
            $"Wait failed after {ShortTime}ms for action: Element appear");
        ShouldThrowInTime(() => _h3.WaitForAppear(ShortTime), ShortTime, ShortTime + TimeMargin,
            $"Wait failed after {ShortTime}ms for action: Element appear");
    }

    [Fact] void WaitForDisappearTest() {
        SpawnHtml(H5Html);
        SpawnHtmlAfterMs("<p>Nothing to do here</p>", Time);

        ShouldEndInTime(() => _h5.WaitForDisappear(), Time, Time + TimeMargin);

        ShouldThrowInTime(() => _p.WaitForDisappear(ShortTime), ShortTime, ShortTime + TimeMargin,
            $"Wait failed after {ShortTime}ms for action: Element disappear:");
    }

    [Fact] void WaitForAnyTest() {
        SpawnHtmlAfterMs(H5Html, Time);

        ShouldEndInTime(() => _longWaiter.WaitForAny(_h3, _h4, _h5), Time, Time + TimeMargin);
        ShouldEqual(_longWaiter.WaitForAny(_h3, _h4, _h5), 2);

        ShouldThrowInTime(() => _shortWaiter.WaitForAny(_h3, _h4), ShortTime, ShortTime + TimeMargin,
            $"Wait failed after {ShortTime}ms for action: Any element of these:");
    }

    [Fact] void WaitForReloadTest() {
        SpawnHtml(H5Html);
        ShouldEndInTime(() => _longWaiter.WaitForReload(_h5,
            () => SpawnHtmlAfterMs(H5Html, Time)), Time, Time + TimeMargin);

        ShouldThrowInTime(() => _shortWaiter.WaitForReload(_h5), ShortTime, ShortTime + TimeMargin,
            $"Wait failed after {ShortTime}ms for action: Reload element:");
    }

    [Fact] void WaitForStablePageTest() {
        S("#redirect input[name=interval]").SetValue("100");
        S("#redirect input[name=count]").SetValue("2");
        S("#redirect button").Click();
        //pge unstable for 200+300ms
        ShouldEndInTime(() => _longWaiter.WaitForStablePage(300), 500, 2000);

        //page is stable
        ShouldEndInTime(() => _longWaiter.WaitForStablePage(100), 100, 100 + TimeMargin);

        S("#redirect button").Click();
        ShouldThrowInTime(() => _shortWaiter.WaitForStablePage(500), ShortTime, ShortTime + TimeMargin,
            $"Page is unstable, loading every <500ms. Timed out after {ShortTime}ms.");
    }

    private void SpawnHtml(string html) {
        ExecuteJs<string>("document.querySelector('#spawn .target').innerHTML = arguments[0]",
            html);
    }

    private void SpawnHtmlAfterMs(string html, int timeoutMs) {
        ExecuteJs<string>(
            "setTimeout(() => document.querySelector('#spawn .target').innerHTML = arguments[0], arguments[1])",
            html, timeoutMs);
    }
}