using System.Text.RegularExpressions;

namespace Comfast.Commons.Utils;

/// <summary> Extension methods for string</summary>
public static class StringExtensions {
    /// <summary> Trim string if is longer than length</summary>
    public static string TrimToMaxLength(this string str, int maxLength) =>
        str.Length <= maxLength ? str : str.Substring(0, maxLength) + "...";
}