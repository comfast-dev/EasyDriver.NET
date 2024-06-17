using Comfast.EasyDriver.Se.Finder;

namespace Comfast.EasyDriver.Se.Locator;

public class SimpleLocator : BaseComponent {
    public override string Selector { get; }
    public override string? Description { get; }

    public SimpleLocator(string selector, string? description = null) {
        Selector = selector;
        Description = description;
    }

    /// <summary>
    /// Add child selector.
    /// e.g. S("html")._S("body")
    /// is equivalent to S("html >> body")
    /// </summary>
    public SimpleLocator _S(string childCssOrXpath, string description = null)
        => new(Selector + WebElementFinder.SelectorSeparator + childCssOrXpath, description);
}