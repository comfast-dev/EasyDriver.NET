namespace Comfast.EasyDriver.Se.Finder;

/// <summary>
/// <para>
/// Accept selectors like:<br/>
/// 1. XPATH: e.g. //html//ul/li[.='My Point']<br/>
///
/// 2. CSS:   e.g. table > td.selected<br/>
///
/// 3. Sub-selectors XPATH>>CSS, where '>>' mean ANY CHILD:<br/>
///  //html//table[1] >> td.selected<br/>
///
/// 4. Any mixed combination like: XPATH>>CSS>>XPATH etc.<br/>
/// //table >> tr.selected >> .//td[.='some text']<br/>
/// </para>
/// </summary>
public static class SelectorChain {
    /// <summary>
    /// used to separate sub-selectors
    /// </summary>
    public const string SelectorSeparator = " >> ";

    /// <summary>
    /// Split chain into array of simple cssOrXpath selectors
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static string[] SplitChain(this string selector) {
        return selector.Split(SelectorSeparator);
    }
}