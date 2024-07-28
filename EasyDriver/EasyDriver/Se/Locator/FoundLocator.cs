using Comfast.EasyDriver.Models;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se.Locator;

/// <summary>
/// Represent Locator already found and fixed with one element.<br/>
/// Using this class is faster, but may cause StaleElementReferenceException.
/// </summary>
public class FoundLocator : SimpleLocator, IFoundLocator {
    /// <summary>
    /// Represent DOM element found in browser
    /// </summary>
    public IWebElement FoundElement { get; }

     public FoundLocator(string selector, string? description, IWebElement foundElement)
        : base(selector, description) {
        FoundElement = foundElement;
    }

    /// <summary>
    /// Override DoFind with same instance
    /// </summary>
    public override IWebElement DoFind() => FoundElement;
}