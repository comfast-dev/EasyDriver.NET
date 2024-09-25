namespace Comfast.EasyDriver.Core.Locator;

/// <summary> Basic locator</summary>
public class SimpleLocator : BaseComponent {
    /// <inheritdoc />
    public override string CssOrXpath { get; }

    /// <inheritdoc />
    public override string Description { get; }

    /// <param name="selector">CSS or XPATH</param>
    /// <param name="description">Locator description for logs</param>
    public SimpleLocator(string selector, string? description) {
        CssOrXpath = selector;
        Description = description ?? "SimpleLocator";
    }
}