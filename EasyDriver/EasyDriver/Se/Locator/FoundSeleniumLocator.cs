using Comfast.EasyDriver.Se.Finder;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se.Locator;

public class FoundSeleniumLocator : SeleniumLocator, IFoundLocator {
    public IWebElement FoundElement { get; }

     public FoundSeleniumLocator(SelectorChain chain, string? description, IWebElement foundElement)
        : base(chain, description) {
        FoundElement = foundElement;
    }

    public override IWebElement DoFind() => FoundElement;
}