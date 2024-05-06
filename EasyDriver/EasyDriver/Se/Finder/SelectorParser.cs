using System.Text.RegularExpressions;
using Comfast.Commons.Utils;

namespace Comfast.EasyDriver.Selector;

public class SelectorParser {
    public const string IsXpathPattern = @"^([\(\.]{0,3}\/|\.\.)";
    
    /**
     * If XPATH passed without dot like "//some/xpath" method will add it like: ".//some/xpath".
     * <p>Explanation: Nested XPATH Selectors should start with dot "./" or ".//", that means "from current node"
     * Otherwise search will is performed from root html element, which isn't expected.</p>
     * @param selector any selector, CSS or XPATH
     * @return same selector, where XPATH is normalized
     */
    public static String NormalizeChildSelector(String selector) {
        return IsXpath(selector)
            ? selector.RgxReplace("^/", "./")
            : selector;
    }
    
    /**
     * @param selector string
     * @return true if selector is XPATH
     */
    public static bool IsXpath(String selector) => Regex.Match(selector, IsXpathPattern).Success;
}