using Comfast.EasyDriver.Se.Finder;

namespace Comfast.EasyDriver.Ui;

public class LinkEl : LabeledComponent {
    public override string[] AllLabels => DriverApi.S("//a").Texts;
    public override SelectorChain Selector => new($"//a[.={Label.EscapeXpath()}]");

    public LinkEl(string label) : base(label) { }
}