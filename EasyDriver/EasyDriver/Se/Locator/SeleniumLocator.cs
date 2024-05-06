using Comfast.EasyDriver.Se.Finder;

namespace Comfast.EasyDriver.Se.Locator;

public class SeleniumLocator : BaseComponent {
    private readonly WebElementFinder _finder;
    public override SelectorChain Chain { get; }
    public override string? Description { get; }

    public SeleniumLocator(string selector, string? description = null)
        : this(new SelectorChain(selector), description) { }

    protected SeleniumLocator(SelectorChain selector, string? description = null) {
        Chain = selector;
        Description = description;

        var driver = Configuration.GetDriver();
        _finder = new WebElementFinder(driver, selector);
    }

    public SeleniumLocator S(string selector, string description = null)
        => new(Chain.Add(selector), description);
}