using System.Collections.ObjectModel;
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
public class WebElementFinder : IFinder<IWebElement> {
    /// <summary>
    /// used to separate sub-selectors
    /// </summary>
    public const string SelectorSeparator = " >> ";

    private readonly IWebDriver _webDriver;
    private readonly string _selector;

    public WebElementFinder(IWebDriver webDriver, string selector) {
        _webDriver = webDriver;
        _selector = selector;
    }

    /// <summary>
    /// Find first matched element
    /// </summary>
    public IWebElement Find() {
        return DoFind(true, _selector.Split(SelectorSeparator));
    }

    /// <summary>
    /// Find all matched elements
    /// </summary>
    public ReadOnlyCollection<IWebElement> FindAll() {
        var selectors = _selector.Split(SelectorSeparator);
        if (selectors.Length == 0) throw new Exception("Empty chain, require at least 1 item");

        var lastBy = String2By(selectors[^1], true);
        if (selectors.Length == 1) return _webDriver.FindElements(lastBy);

        var parents = selectors.SkipLast(1).ToArray();
        var parent = DoFind(true, parents);
        return parent.FindElements(lastBy);
    }

    private IWebElement DoFind(bool throwIfNotFound, params string[] selectors) {
        int i = 0;
        try {
            var currentEl = _webDriver.FindElement(String2By(selectors[i], false));
            for (i = 1; i < selectors.Length; i++) {
                currentEl = FindChild(currentEl, selectors[i]);
            }

            return currentEl;
            // } catch(InvalidSelectorException | NoSuchElementException | InvalidArgumentException | JavascriptException ex) {
        } catch (Exception ex) {
            if (!throwIfNotFound) return null!;
            throw new ElementFindFail(selectors, i, ex);
        }
    }

    /// <summary>
    /// Find child of parent element, includes shadow dom
    /// </summary>
    private IWebElement FindChild(ISearchContext parent, string selector) {
        By by = String2By(selector, true);
        try {
            return parent.FindElement(by);
        } catch (NoSuchElementException) {
            //handle case where child element is under shadow DOM
            var jsDriver = (IJavaScriptExecutor)_webDriver;
            var shadowRoot = (ISearchContext)jsDriver.ExecuteScript("return arguments[0].shadowRoot", parent);
            if (shadowRoot == null) throw;
            return shadowRoot.FindElement(by);
        }
    }

    private By String2By(string selector, bool normalizeXpath) {
        return selector.IsXpath()
            ? By.XPath(normalizeXpath ? Xpath.NormalizeChildXpath(selector) : selector)
            : By.CssSelector(selector);
    }
}