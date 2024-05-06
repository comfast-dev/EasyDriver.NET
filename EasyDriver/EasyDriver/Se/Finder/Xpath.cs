using System.Text;
using System.Text.RegularExpressions;

namespace Comfast.EasyDriver.Se.Finder;

public static class Xpath {
    /**
     * Escape text for usage in xpath, so it can include ' and " characters
     * how to use:
     * string xpath = $"//a[text()={EscapeXpath("It's \"hard\" text to match")}]"
     */
    public static string EscapeXpath(this string text) {
        var parts = text.Split("'");
        if (parts.Length == 1) return $"'{text}'";
        if (!text.Contains('"')) return $"\"{text}\"";

        var builder = new StringBuilder("concat('");
        for (var i = 0; i < parts.Length; i++) {
            builder.Append(parts[i] + "'");
            if (i < parts.Length - 1) builder.Append(", \"'\", '");
        }

        return builder.Append(')').ToString();
    }

    public static bool IsXpath(this string cssXpathChain) {
        return Regex.IsMatch(cssXpathChain, @"[\.\(]*/");
    }
}