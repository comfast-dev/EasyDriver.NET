using Comfast.EasyDriver.Se.Locator;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Ui;

/// <summary>
/// Used to define component that can be defined by some "Label" visible on page.
/// Example Implementation <see cref="LinkByText"/>
/// </summary>
public abstract class LabeledComponent : BaseComponent {
    /// <summary> return all labels on current page that can be used to create component</summary>
    public abstract string[] AllLabels { get; }

    /// <summary> Label used to find the element (construct the Selector)</summary>
    public string Label { get; }

    /// <summary> Description for logging</summary>
    public override string Description => $"{GetType().Name}: {Label}";

    public LabeledComponent(string label) {
        Label = label;
    }

    /// <summary> Add details to error message</summary>
    public override IWebElement FindElement() {
        try {
            return base.FindElement();
        } catch (Exception e) {
            var labels = string.Join("\", \"", AllLabels);
            throw new(e.Message + $"\nAvailable {GetType().Name} labels are: \"{labels}\" \n");
        }
    }
}