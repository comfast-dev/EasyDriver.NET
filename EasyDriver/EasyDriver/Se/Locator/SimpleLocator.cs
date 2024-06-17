using Comfast.EasyDriver.Se.Finder;

namespace Comfast.EasyDriver.Se.Locator;

/// <summary>
/// Basic locator
/// </summary>
public class SimpleLocator : BaseComponent {
    /// <inheritdoc />
    public override string Selector { get; }

    /// <inheritdoc />
    public override string? Description { get; }

    /// <param name="selector">CSS or XPATH</param>
    /// <param name="description">Locator description for logs</param>
    public SimpleLocator(string selector, string? description = null) {
        Selector = selector;
        Description = description;
    }
}