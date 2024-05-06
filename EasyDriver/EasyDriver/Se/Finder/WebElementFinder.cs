using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se.Finder;

public class WebElementFinder : IFinder<IWebElement> {
    private readonly IWebDriver _webDriver;
    private readonly SelectorChain _chain;

    public WebElementFinder(IWebDriver webDriver, SelectorChain chain) {
        _webDriver = webDriver;
        _chain = chain;
    }

    public IWebElement Find() {
        return DoFind(true, _chain.Split());
    }

    public ReadOnlyCollection<IWebElement> FindAll() {
        var selectors = _chain.Split();
        if (selectors.Length == 0) throw new Exception("Empty chain, require at least 1 item");

        var lastBy = String2By(selectors[selectors.Length - 1]);
        if (selectors.Length == 1) return _webDriver.FindElements(lastBy);

        var parents = selectors.SkipLast(1).ToArray();
        var parent = DoFind(true, parents);
        return parent.FindElements(lastBy);
    }

    private IWebElement DoFind(bool throwIfNotFound, params string[] selectors) {
        int i = 0;
        try {
            var currentEl = _webDriver.FindElement(String2By(selectors[i]));
            for (i = 1; i < selectors.Length; i++) {
                currentEl = FindChild(currentEl, String2By(selectors[i]));
            }

            return currentEl;
            // } catch(InvalidSelectorException | NoSuchElementException | InvalidArgumentException | JavascriptException ex) {
        } catch (Exception ex) {
            if (!throwIfNotFound) return null;
            // throw new ElementFindFail(chain, i, ex);
            throw new Exception("Element find fail", ex);
        }
    }

    private By String2By(String selector) {
        return SelectorParser.IsXpath(selector)
            ? By.XPath(selector)
            : By.CssSelector(selector);
    }

    /**
     * Find child of parent element, includes shadow dom
     */
    private IWebElement FindChild(ISearchContext parent, By by) {
        try {
            return parent.FindElement(by);
        } catch (NoSuchElementException) {
            //handle case where next element is under shadow DOM
            var jsDriver = (IJavaScriptExecutor)_webDriver;
            var shadowRoot = (ISearchContext)jsDriver.ExecuteScript("return arguments[0].shadowRoot", parent);
            if (shadowRoot == null) throw;
            return shadowRoot.FindElement(by);
        }
    }
}