namespace Comfast.EasyDriver.Core.Finder;

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
public class SelectorChain {
    /// <summary> used to separate sub-selectors</summary>
    public const string SelectorSeparator = " >> ";

    private readonly string _selectorString;

    public SelectorChain(string selectorString) {
        _selectorString = selectorString;
    }

    public string[] SelectorsArray => _selectorString.Split(SelectorSeparator);
}