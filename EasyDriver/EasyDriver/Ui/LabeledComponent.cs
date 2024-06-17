using Comfast.EasyDriver.Se.Locator;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Ui;

/// <summary>
/// Example Implementation <see cref="LinkByText"/>
/// </summary>
public abstract class LabeledComponent : BaseComponent {
    public abstract string[] AllLabels { get; }

    public string Label { get; }
    public override string Description => $"{GetType().Name}: {Label}";

    public LabeledComponent(string label) {
        Label = label;
    }

    public override IWebElement DoFind() {
        try {
            return base.DoFind();
        } catch (Exception e) {
            var labels = string.Join("\", \"", AllLabels);
            throw new(e.Message + $"\nAvailable {GetType().Name} labels are: \"{labels}\" \n");
        }
    }
}