namespace Comfast.EasyDriver.Se.Finder;

/// <summary>
/// Throw when not found DOM element.
/// </summary>
public class ElementFindFail : Exception {
    public ElementFindFail(string[] selectors, int failIndex, Exception cause)
        : base(BuildErrorMessage(selectors, failIndex, cause), cause) { }

    private static string BuildErrorMessage(string[] selectors, int failIndex, Exception cause) {
        var separator = SelectorChain.SelectorSeparator;
        var offset = selectors.Take(failIndex).Aggregate(0, (acc, x) => acc + x.Length + separator.Length);
        var spaces = new String(' ', offset);

        return $"\nElement find fail:"
               + $"\n{string.Join(separator, selectors)}\n{spaces}^\n{spaces}{cause.Message}";
    }
}