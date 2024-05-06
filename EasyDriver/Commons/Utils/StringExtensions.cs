using System.Text.RegularExpressions;

namespace Comfast.Commons.Utils;

public static class StringExtensions {
    public static string LimitString(this string input, int maxLength) {
        return input.Length > maxLength
            ? input.Substring(0, maxLength) + "..."
            : input;
    }

    public static string RgxReplace(this string str, string pattern, string replacement) {
        return Regex.Replace(str, pattern, replacement);
    }

    public static string? RgxMatch(this string str, string pattern, int capturingGroup = 0) {
        var match = Regex.Match(str, pattern);
        
        return match.Success 
            ? match.Groups[capturingGroup].Value 
            : null;
    }
    
    public static string MaxLength(this string str, int length) {
        return str.Length <= length ? str : str.Substring(0, length);
    }
}