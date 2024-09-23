using OpenQA.Selenium;

namespace Comfast.EasyDriver.Models;

/// <summary> Represents DOM Element already found.</summary>
public interface IFoundLocator : ILocator {
    /// <summary> Instance of WebDriver Element</summary>
    public IWebElement FoundWebElement { get; }
}