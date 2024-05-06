using OpenQA.Selenium;

namespace Comfast.EasyDriver;

public interface IFoundLocator : ILocator {
    public IWebElement FoundElement { get; }
}