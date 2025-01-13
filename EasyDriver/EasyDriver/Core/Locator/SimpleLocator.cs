namespace Comfast.EasyDriver.Core.Locator;

/// <summary> Basic ILocator implementation.</summary>
public class SimpleLocator : BaseComponent {
    /// <inheritdoc />
    public override string CssOrXpath { get; }

    /// <inheritdoc />
    public override sealed string Description { get; protected set; }

    /// <param name="selector">CSS or XPATH</param>
    public SimpleLocator(string selector) {
        CssOrXpath = selector;
        Description = "";
    }
}