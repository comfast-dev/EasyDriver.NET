using System.Globalization;
using System.Text;
using Comfast.Commons.Utils;

namespace Comfast.EasyDriver.Ui;

/// <summary>
/// Extension to recognize elements of type: LabeledComponenet
/// </summary>
public static class LabeledComponentExtensions {
    private static readonly string[] Colors = { "red", "green", "blue", "yellow", "orange", "pink", "cyan", "magenta" };

    /// <summary>
    /// Recognize given elements by highlighting them and prepare console summary.
    /// </summary>
    /// <param name="types">List of LabeledComponent types to recognize.</param>
    /// <returns>string summary for print in console</returns>
    public static string RecognizeComponents(IEnumerable<Type> types) {
        int i = 0;
        StringBuilder str = new();
        foreach (var type in types) {
            if (!type.IsSubclassOf(typeof(LabeledComponent))) throw new Exception(
                $"Require {type} to be {typeof(LabeledComponent)}");

            var color = Colors[i++ % Colors.Length];
            str.Append($"\r\n{type.Name} ({color}):\r\n")
                .Append(CreateInstance(type, "").RecognizeComponent(color))
                .Append("\r\n");
        }

        return str.ToString();;
    }

    /// <summary>
    /// Recognize all elements of LabeledComponent type
    /// </summary>
    /// <param name="component">instance</param>
    /// <param name="highlightColor">if not null - add border in this color</param>
    /// <returns>Console log</returns>
    public static string RecognizeComponent(this LabeledComponent component, string? highlightColor = null) {
        var type = component.GetType();
        return string.Join("\n", component.AllLabels
            .Where(label => label.Trim().Length > 0)
            .Select(label => CreateInstance(type, label))
            .Where(el => el.IsDisplayed)
            .Select(el => {
                if (highlightColor != null) el.Highlight(highlightColor);
                var fieldName = el.Label.ToPascalCase().MaxLength(25);
                return $"{type.Name} {fieldName} => new(\"{el.Label}\");";
            }));
    }

    private static LabeledComponent CreateInstance(Type type, string label) {
        return (LabeledComponent)Activator.CreateInstance(type, label)!
               ?? throw new InvalidOperationException();
    }

    private static string ToPascalCase(this string input) {
        var myTi = new CultureInfo("en-US", false).TextInfo;

        var cleanInput = input.RgxReplace("[^A-Za-z ]", "");
        return myTi.ToTitleCase(cleanInput).RgxReplace(" +", "");
    }
}