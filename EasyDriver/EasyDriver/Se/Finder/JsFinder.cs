using OpenQA.Selenium;

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
public class JsFinder : IFinder<IWebElement> {
    private readonly IJavaScriptExecutor _jsDriver;
    private readonly string[] _selectors;

    public JsFinder(IWebDriver webDriver, string selector) {
        _jsDriver = (IJavaScriptExecutor)webDriver;
        _selectors = selector.SplitChain();
    }

    //language=JavaScript
    private readonly string _js = @"
const isXpath = cssOrXpath => /^([\(\.]{0,3}\/|\.\.)/.test(cssOrXpath)
const fixXpath = xpath => xpath.replace(/^\//, ""./"")

const any$ = (cssOrXpath, parent = document) => !isXpath(cssOrXpath)
        ? parent.querySelector(cssOrXpath)
        : document.evaluate(fixXpath(cssOrXpath), parent, null, 9, null).singleNodeValue

/**
* @param selectors {string[]}
* @returns {(number|*)[]|Document}
*/
function find(selectors) {
    let curr = document;
    for (let i = 0; i < selectors.length; i++) {
        let parent = curr;
        if (curr.shadowRoot) parent = curr.shadowRoot;
        if (curr.contentWindow) parent = curr.contentWindow.document

        curr = any$(selectors[i], parent)
        if(curr == null) return i;
    }
    return curr;
}
";

    /// <summary>
    /// Find first matched element
    /// </summary>
    public IWebElement Find() {
        var result = _jsDriver.ExecuteScript(_js + "return find(arguments)", _selectors);
        if (result is IWebElement foundElement) return foundElement;

        throw new ElementFindFail(_selectors, (int)(long)result, new Exception("Not found"));
    }

    /// <summary>
    /// Find all matched elements
    /// </summary>
    public IList<IWebElement> FindAll() {
        var result = _jsDriver.ExecuteScript(_js + "return find(arguments)", _selectors);
        if (result is IWebElement[] foundElements) return new List<IWebElement>(foundElements);

        throw new Exception("Internal error: Invalid return from Javascript console");
    }
}