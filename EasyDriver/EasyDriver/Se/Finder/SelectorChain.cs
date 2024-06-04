namespace Comfast.EasyDriver.Se.Finder;

/// <summary>
/// Represent XPATH/CSS selector e.g.
/// //html//ul/li[.='My Point']
/// ul > li.selected
///
/// Or Chain of selectors separated by ' >> ' (can mix XPATH/CSS)
/// table tr.selected >> .//td[.='some text']
/// //html//table/tr >> td.selected
/// </summary>
public class SelectorChain {
    public const string Separator = " >> ";
    private readonly string _chain;

    public SelectorChain(string cssOrXpathChain) {
        _chain = cssOrXpathChain;
    }

    public SelectorChain Add(string selector) {
        return new SelectorChain(_chain + Separator + SelectorParser.NormalizeChildSelector(selector));
    }

    public string[] Split() => _chain.Split(Separator);
    public override string ToString() => _chain;
}