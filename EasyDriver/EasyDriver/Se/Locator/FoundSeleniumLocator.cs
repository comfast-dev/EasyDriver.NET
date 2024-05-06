using Comfast.EasyDriver.Locator;
using Comfast.EasyDriver.Selector;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se;

public class FoundSeleniumLocator : SeleniumLocator, IFoundLocator {
    public IWebElement FoundElement { get; }

    public FoundSeleniumLocator(SelectorChain chain, string? description, IWebElement foundElement) 
        : base(chain, description) {
        FoundElement = foundElement;
    }
    
    protected override IWebElement DoFind() => FoundElement;
}