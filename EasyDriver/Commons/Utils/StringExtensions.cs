using System.Text.RegularExpressions;

namespace Comfast.Commons.Utils;

/// <summary>
/// Extension methods for string
/// </summary>
public static class StringExtensions {
    /// <summary>
    /// Replace regex matches with given replacement.
    /// </summary>
    public static string RgxReplace(this string str, string pattern, string replacement) =>
        Regex.Replace(str, pattern, replacement);

    /// <summary>
    /// Match regex pattern. If matchGroup is set use nth capturing group.
    /// </summary>
    public static string? RgxMatch(this string str, string pattern, int capturingGroup = 0) {
        var match = Regex.Match(str, pattern);
        return match.Success ? match.Groups[capturingGroup].Value : null;
    }

    /// <summary>
    /// Trim string if is longer than length
    /// </summary>
    public static string TrimToMaxLength(this string str, int maxLength) =>
        str.Length <= maxLength ? str : str.Substring(0, maxLength) + "...";
}