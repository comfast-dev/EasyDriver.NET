using Comfast.EasyDriver.Se;
using Comfast.EasyDriver.Se.Locator;

namespace Comfast.EasyDriver.Ui;

/// <summary>
/// Example Implementation <see cref="LinkEl"/>
/// </summary>
public abstract class LabeledComponent : BaseComponent {
    public abstract string[] AllLabels { get; }

    public string Label { get; }
    public override string Description => $"{GetType().Name}: {Label}";

    public LabeledComponent(string label) {
        Label = label;
    }

    public override IFoundLocator Find() {
        try {
            return base.Find();
        } catch (Exception e) {
            var labels = string.Join("\", \"", AllLabels);
            throw new(e.Message + $"\nAvailable {GetType().Name} labels are: \"{labels}\" \n");
        }
    }
}