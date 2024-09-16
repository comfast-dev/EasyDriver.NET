using System.Globalization;
using System.Text;
using Comfast.Commons.Utils;

namespace Comfast.EasyDriver.Ui;

/// <summary> Extension to recognize elements of type: LabeledComponent</summary>
public static class LabeledComponentUtils {
    /// <summary> Find all components of given type in current page</summary>
    public static T[] FindAll<T>() where T : LabeledComponent => (T[])FindAll(typeof(T));

    /// <summary> Find all components of given type in current page</summary>
    public static LabeledComponent[] FindAll(Type type) =>
        CreateInstance(type, "").AllLabels
            .Where(label => label.Trim().Length > 0)
            .Select(label => CreateInstance(type, label))
            .Where(el => el.IsDisplayed)
            .ToArray();

    /// <summary> Recognize given elements by highlighting them and prepare console summary.</summary>
    /// <param name="types">List of LabeledComponent types to recognize.</param>
    /// <returns>string summary for print in console</returns>
    public static string RecognizeTypes(params Type[] types) {
        string[] colors = { "red", "green", "blue", "yellow", "orange", "pink", "cyan", "magenta" };

        int i = 0;
        StringBuilder str = new();
        foreach (var type in types) {
            var color = colors[i++ % colors.Length];
            str.Append($"\r\n{type.Name} ({color}):\r\n")
                .Append(RecognizeType(type, color))
                .Append("\r\n");
        }

        return str.ToString();
    }

    /// <summary> Recognize all elements of LabeledComponent type</summary>
    /// <param name="type">LabeledComponent type</param>
    /// <param name="highlightColor">if set - add border in this color to component</param>
    /// <returns>string summary for print in console</returns>
    private static string RecognizeType(Type type, string? highlightColor = null) {
        return string.Join("\n",
            FindAll(type).Select(el => {
                if (highlightColor != null) el.Highlight(highlightColor);
                var fieldName = el.Label.ToPascalCase().TrimToMaxLength(25);
                return $"{type.Name} {fieldName} = new(\"{el.Label}\");";
            }));
    }

    private static LabeledComponent CreateInstance(Type type, string label) {
        if (!type.IsSubclassOf(typeof(LabeledComponent)))
            throw new Exception(
                $"Require {type} to be {typeof(LabeledComponent)}");

        return (LabeledComponent)Activator.CreateInstance(type, label)!;
    }

    private static string ToPascalCase(this string input) {
        var myTi = new CultureInfo("en-US", false).TextInfo;

        var cleanInput = input.RgxReplace("[^A-Za-z ]", "");
        return myTi.ToTitleCase(cleanInput).RgxReplace(" +", "");
    }
}