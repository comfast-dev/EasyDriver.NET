using Comfast.EasyDriver.Core.Finder;

namespace Comfast.EasyDriver.Lib;

/// <summary> Example implementation of LabeledComponent</summary>
public class LinkByText : LabeledComponent {
    public override string[] AllLabels => S("//a").Texts;
    public override string CssOrXpath => $"//a[.={Label.EscapeQuotesInXpath()}]";

    public LinkByText(string label) : base(label) { }
}