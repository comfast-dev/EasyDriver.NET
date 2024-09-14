using System.Text.RegularExpressions;
using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Finder;
using Comfast.EasyDriver.Ui;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using static Comfast.EasyDriver.DriverApi;

namespace Comfast.EasyDriver.Se.Locator;

/// <summary>
/// Base class for component.
/// Example implementation:
/// <see cref="LinkByHref"/>
/// </summary>
public abstract class BaseComponent : ILocator {
    /// <summary>
    /// Css / Xpath selector<br/>
    /// or <i>Selector >> SubSelector >> ...</i> chain delimited with ' >> '
    ///
    /// <example><code>
    /// XPATH: "//input[@name='user']"
    /// CSS: "form.focused"
    /// CHAIN: "form.focused >> //input[@name='user'] >> //span"
    /// </code></example>
    /// </summary>
    public abstract string Selector { get; }

    /// <summary>
    /// Description for this Component / Locator
    /// Used in logs / error messages
    /// </summary>
    public abstract string Description { get; }

    /// <inheritdoc />
    public ILocator _S(string cssOrXpath, string? description) => SubLocator(cssOrXpath, description);

    /// <inheritdoc />
    public ILocator SubLocator(string cssOrXpath, string? description) {
        return new SimpleLocator(Selector + SelectorChain.SelectorSeparator + cssOrXpath, description);
    }

    /// <inheritdoc />
    public virtual IWebElement FindElement() {
        return WebElementFinder.Find();
    }

    /// <inheritdoc />
    public virtual IList<IWebElement> FindElements() {
        return WebElementFinder.FindAll();
    }

    /// <inheritdoc />
    public virtual IFoundLocator Find() {
        return new FoundLocator(Selector, Description, FindElement());
    }

    /// <inheritdoc />
    public virtual IList<IFoundLocator> FindAll() {
        return FindElements()
            .Select(webEl => (IFoundLocator)new FoundLocator(Selector, Description, webEl))
            .ToList();
    }

    /// <inheritdoc />
    public virtual IFoundLocator? TryFind() {
        try {
            return Find();
        } catch (Exception) {
            return null;
        }
    }

    /// <inheritdoc />
    public virtual IFoundLocator Nth(int number) {
        if (number < 1) throw new Exception($"Invalid number: {number}. Nth is indexed from 1");
        var all = FindElements();
        if (all.Count < number)
            throw new Exception($"Not found element #{number}. There are {all.Count} matched by:\n{Selector}");

        return new FoundLocator(Selector, Description, all[number - 1]);
    }

    /// <inheritdoc />
    public virtual string Text => ExecuteJs<string>("return el.innerText");

    /// <inheritdoc />
    public virtual string[] Texts =>
        Configuration.LocatorConfig.Experimental.GetTextsUsingJs
            ? MapUsingJs<string>("return el.innerText").ToArray()
            : Map(el => el.Text).ToArray();

    /// <inheritdoc />
    public virtual int Count => FindAll().Count;

    /// <inheritdoc />
    public virtual bool IsDisplayed => TryGet(el => el.Displayed);

    /// <inheritdoc />
    public virtual bool Exists => TryFind() != null;

    /// <inheritdoc />
    public virtual string TagName => FindElement().TagName;

    /// <inheritdoc />
    public virtual string InnerHtml => GetAttribute("innerHTML");

    /// <inheritdoc />
    public virtual string OuterHtml => GetAttribute("outerHTML");

    /// <inheritdoc />
    public virtual string Value => GetAttribute("value");

    /// <inheritdoc />
    public string DomId =>
        FindElement().ReadField<string>("elementId")
        ?? throw new("Fatal error: field elementId is null");

    /// <inheritdoc />
    public virtual bool HasClass(string cssClass) {
        return GetAttribute("class").Split(" ").Contains(cssClass);
    }

    /// <inheritdoc />
    public virtual string GetCssValue(string cssPropertyName) {
        return FindElement().GetCssValue(cssPropertyName);
    }

    /// <inheritdoc />
    public virtual string GetAttribute(string name) {
        return FindElement().GetAttribute(name);
    }

    /// <inheritdoc />
    public bool HasAttribute(string name) {
        return FindElement().GetAttribute(name) != null;
    }

    /// <inheritdoc />
    public virtual ILocator Click() {
        HighlightIfEnabled();
        FindElement().Click();
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator SendKeys(string text) {
        HighlightIfEnabled();
        FindElement().SendKeys(text);
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator SetValue(string text) {
        var found = FindElement();
        var tag = found.TagName;
        if (tag != "input" && tag != "textarea")
            throw new NotImplementedException($"SetValue doesn't handle {tag}. Handle only input / textarea.");

        var el = found;
        HighlightIfEnabled();
        el.Clear();
        el.SendKeys(text);
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator Highlight(string cssColor = "red") {
        ExecuteJs($"el.style.border = '2px solid {cssColor}'");
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator Hover() {
        HighlightIfEnabled();
        new Actions(GetDriver()).MoveToElement(FindElement()).Perform();
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator Focus() {
        HighlightIfEnabled();
        ExecuteJs("el.focus()");
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator Clear() {
        HighlightIfEnabled();
        FindElement().Clear();
        return this;
    }

    /// <inheritdoc />
    public ILocator ScrollIntoView() {
        return ExecuteJs("el.scrollIntoView({behavior: 'smooth'})");
    }

    /// <inheritdoc />
    public virtual ILocator DragTo(ILocator target) {
        HighlightIfEnabled();
        IWebElement targetEl = target.FindElement();
        ExecuteJs(ReadJsFile("dragAndDrop.js") + "executeDragAndDrop(el, arguments[0])", targetEl);

        return this;
// Selenium native implementation (causes problems)
// new Actions(getDriver()).dragAndDrop(find(), targetEl).perform();
    }

    /// <inheritdoc />
    public ILocator ExecuteJs(string jsCode, params object[] jsArgs) {
        var jsDriver = (IJavaScriptExecutor)GetDriver();

        jsArgs = new List<object>(jsArgs) { FindElement() }.ToArray();
        jsDriver.ExecuteScript($"const el = arguments[{jsArgs.Length - 1}];{jsCode}", jsArgs);
        return this;
    }

    /// <inheritdoc />
    public TReturnType ExecuteJs<TReturnType>(string jsCode, params object[] jsArgs) {
        var jsDriver = (IJavaScriptExecutor)GetDriver();

        // add return statement for trivial scripts if it's missing e.g. "el.value" => "return el.value"
        if (!Regex.IsMatch(jsCode, @"(\n|;|return|}|{)")) {
            jsCode = "return " + jsCode;
        }

        jsArgs = new List<object>(jsArgs) { FindElement() }.ToArray();
        var result = jsDriver.ExecuteScript($"const el = arguments[{jsArgs.Length - 1}];{jsCode}", jsArgs);
        return (TReturnType)result;
    }

    /// <inheritdoc />
    public ILocator WaitFor(int? timeoutMs = null, bool throwIfFail = true) {
        try {
            return GetWaiter(timeoutMs).WaitFor("Element exist.", Find);
        } catch (Exception) {
            if (throwIfFail) throw;
            return this;
        }
    }

    /// <inheritdoc />
    public virtual ILocator WaitForAppear(int? timeoutMs = null, bool throwIfFail = true) {
        try {
            GetWaiter(timeoutMs).WaitFor($"Element appear: \n{Selector}", () => Find().IsDisplayed);
        } catch (Exception) {
            if (throwIfFail) throw;
        }

        return this;
    }

    /// <inheritdoc />
    public virtual ILocator WaitForDisappear(int? timeoutMs = null) {
        GetWaiter(timeoutMs).WaitFor($"Element disappear: \n{Selector}", () => !IsDisplayed);
        return this;
    }

    /// <summary>
    /// Map every found element using JavaScript code.
    /// </summary>
    /// <param name="jsMappingCode"> js code where current element is defined as: 'el'</param>
    /// <typeparam name="T">Return type of js code</typeparam>
    /// <returns>List of all mapped elements</returns>
    public virtual IList<T> MapUsingJs<T>(string jsMappingCode) {
        return JsFinder.FindAllAndMap<T>(jsMappingCode);
    }

    /// <inheritdoc />
    public virtual List<T> Map<T>(Func<IFoundLocator, T> func) {
        var elements = FindAll().ToList();
        var results = new List<T>();

        for (int i = 0; i < elements.Count; i++) {
            var element = elements[i];
            try {
                results.Add(func.Invoke(element));
            } catch (Exception e) {
                var elementHtml = element.OuterHtml.MaxLength(100);
                throw new Exception($"Mapping failed during processing element [{i}/{elements.Count}]: " + elementHtml,
                    e);
            }
        }

        return results;
    }

    /// <inheritdoc />
    public override string ToString() => Description;

    private string ReadJsFile(string jsFileName) {
        return File.ReadAllText("EasyDriver\\Js\\" + jsFileName);
    }

    private WebElementFinder WebElementFinder => new(GetDriver(), Selector);
    private JsFinder JsFinder => new(GetDriver(), Selector);

    private void HighlightIfEnabled() {
        if (Configuration.LocatorConfig.HighlightActions) {
            var found = TryFind();
            if (found != null) found.Highlight();
        }
    }

    private T? TryGet<T>(Func<IWebElement, T> func) {
        try {
            return func.Invoke(FindElement());
        } catch (ElementFindFail) {
            return default;
        } catch (StaleElementReferenceException) {
            return default;
        }
    }
}