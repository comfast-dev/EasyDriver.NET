using System.Text;
using System.Text.RegularExpressions;
using Comfast.Commons.Utils;

namespace Comfast.EasyDriver.Se.Finder;

public static class Xpath {
    public const string IsXpathPattern = @"^([\(\.]{0,3}\/|\.\.)";
    public static bool IsXpath(this string cssOrXpath) => Regex.IsMatch(cssOrXpath, IsXpathPattern);

    /// <summary>
    /// Escape text for usage in xpath, so it can include ' and " characters
    /// how to use:
    /// string xpath = $"//a[text()={EscapeXpath("It's \"hard\" text to match")}]"
    /// </summary>
    public static string EscapeQuotesInXpath(this string text) {
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

    /// <summary>
    /// If child XPATH is passed without dot like: "//some/xpath" method will add dot: ".//some/xpath".
    /// Explanation: Nested XPATH Selectors should start with dot "./" or ".//", that means "from current node"
    /// Otherwise search will is performed from root html element, which isn't expected here
    /// </summary>
    /// <param name="selector">selector any selector, CSS or XPATH</param>
    /// <returns>same selector, where XPATH is normalized</returns>
    public static String NormalizeChildXpath(string selector) {
        return IsXpath(selector)
            ? selector.RgxReplace("^/", "./")
            : selector;
    }
}