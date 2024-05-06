using System.Globalization;
using System.Text;
using Comfast.Commons.Utils;

namespace Comfast.EasyDriver.Ui;

public static class ComponentExtensions {
    private static readonly string[] Colors = { "red", "green", "blue", "yellow", "orange", "pink", "cyan", "magenta" };

    public static string RecognizeTypes(IEnumerable<Type> types, bool printOnConsole = true) {
        int i = 0;
        StringBuilder str = new();
        foreach (var type in types) {
            if (!type.IsAssignableFrom(typeof(LabeledComponent)))
                throw new Exception(
                    $"Require {type} to be {typeof(LabeledComponent)}");

            var color = Colors[i++ % Colors.Length];
            str.Append($"\r\n{type.Name} ({color}):\r\n")
                .Append(CreateInstance(type, "").RecognizeElements(color))
                .Append("\r\n");
        }

        var result = str.ToString();
        if (printOnConsole) Console.WriteLine(result);
        return result;
    }

    public static string RecognizeElements(this LabeledComponent component, string? highlightColor = null) {
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