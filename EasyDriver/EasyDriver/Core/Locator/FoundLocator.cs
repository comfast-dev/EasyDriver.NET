using Comfast.EasyDriver.Models;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Core.Locator;

/// <summary>
/// Represent Locator already found and fixed with one element.<br/>
/// Using this class is faster, but may cause StaleElementReferenceException.
/// </summary>
public class FoundLocator : SimpleLocator, IFoundLocator {
    /// <summary> Represent DOM element found in browser</summary>
    public IWebElement FoundWebElement { get; }

    public FoundLocator(ILocator locator, IWebElement foundElement) : base(locator.CssOrXpath) {
        FoundWebElement = foundElement;
        As("Found: " + locator.Description);
    }

    /// <summary> Override DoFind with same instance</summary>
    public override IWebElement FindWebElement() => FoundWebElement;
}