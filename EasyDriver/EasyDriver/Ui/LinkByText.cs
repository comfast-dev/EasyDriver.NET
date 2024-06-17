using Comfast.EasyDriver.Se.Finder;

namespace Comfast.EasyDriver.Ui;

/// <summary>
/// Example implementation of LabeledComponent
/// </summary>
public class LinkByText : LabeledComponent {
    public override string[] AllLabels => DriverApi.S("//a").Texts;
    public override string Selector => $"//a[.={Label.EscapeXpath()}]";

    public LinkByText(string label) : base(label) { }
}