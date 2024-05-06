using Comfast.EasyDriver.Locator;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se;

public interface IFoundLocator : ILocator {
    public IWebElement FoundElement { get; }
}