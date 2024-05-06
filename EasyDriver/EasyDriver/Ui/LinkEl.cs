using Comfast.EasyDriver.Locator;
using Comfast.EasyDriver.Selector;

namespace Comfast.EasyDriver.Ui;

public class LinkEl : LabeledComponent {
    public override string[] AllLabels => CfApi.S("//a").Texts;
    public override SelectorChain Chain => new($"//a[.={Label.EscapeXpath()}]");
    
    public LinkEl(string label) : base(label) { }
}