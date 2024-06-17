using Comfast.EasyDriver.Se.Finder;
using Comfast.EasyDriver.Se.Locator;

namespace Comfast.EasyDriver.Ui;

/// <summary>
/// Example implementation of Component
/// </summary>
public class LinkByHref : BaseComponent {
    private readonly string _href;

    public override string Selector => $"a[href='{_href}']";
    public override string? Description => "My link to: " + _href;

    public LinkByHref(string href) {
        _href = href;
    }
}