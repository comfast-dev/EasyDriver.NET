using Comfast.EasyDriver.Models;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Core.Infra.Browser;

internal class SimpleBrowserRunner : IBrowserRunner {
    private readonly Func<IWebDriver> _runFunc;

    public SimpleBrowserRunner(Func<IWebDriver> runFunc) {
        _runFunc = runFunc;
    }

    public IWebDriver RunNewBrowser() => _runFunc();
}