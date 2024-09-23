using System.Text.RegularExpressions;
using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Errors;
using Comfast.EasyDriver.Se.Finder;
using Comfast.EasyDriver.Ui;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Comfast.EasyDriver.Se.Locator;

/// <summary>
/// Base class for component.
/// Example implementation:
/// <see cref="LinkByHref"/>
/// </summary>
public abstract class BaseComponent : ILocator {
    /// <summary>
    /// Css / Xpath / both delimited by ' >> '<br/>
    /// <example><code>
    /// XPATH: "//input[@name='user']"
    /// CSS: "form.focused"
    /// BOTH: "form.focused >> //input[@name='user'] >> //span"
    /// </code></example>
    /// </summary>
    public abstract string CssOrXpath { get; }

    /// <summary> Optional metadata, used in logs and error messages. </summary>
    public abstract string Description { get; }

    /// <inheritdoc />
    public ILocator _S(string cssOrXpath, string? description) => SubLocator(cssOrXpath, description);

    /// <inheritdoc />
    public ILocator SubLocator(string cssOrXpath, string? description) {
        return new SimpleLocator(CssOrXpath + SelectorChain.SelectorSeparator + cssOrXpath, description);
    }

    /// <inheritdoc />
    public virtual IWebElement FindWebElement() => CallAction("FindElement", () => {
        return WebElementFinder.Find();
    });

    /// <inheritdoc />
    public virtual IList<IWebElement> FindElements() => CallAction("FindElements", () => {
        return WebElementFinder.FindAll();
    });

    /// <inheritdoc />
    public virtual IFoundLocator Find() => CallAction("Find", () => {
        return new FoundLocator(CssOrXpath, Description, FindWebElement());
    });

    /// <inheritdoc />
    public virtual IList<IFoundLocator> FindAll() => CallAction("FindAll", () => {
        return FindElements()
            .Select(webEl => (IFoundLocator)new FoundLocator(CssOrXpath, Description, webEl))
            .ToList();
    });

    /// <inheritdoc />
    public virtual IFoundLocator? TryFind() => CallAction("TryFind", () => {
        try {
            return Find();
        } catch (Exception) {
            return null;
        }
    });

    /// <inheritdoc />
    public virtual IFoundLocator Nth(int number) => CallAction("Nth", () => {
        if (number < 1) throw new Exception($"Invalid number: {number}. Nth is indexed from 1");
        var all = FindElements();
        if (all.Count < number)
            throw new Exception($"Not found element #{number}. There are {all.Count} matched by:\n{CssOrXpath}");

        return new FoundLocator(CssOrXpath, Description, all[number - 1]);
    });

    /// <inheritdoc />
    public virtual string Text => ExecuteJs<string>("return el.innerText");

    /// <inheritdoc />
    public virtual string[] Texts =>
        Configuration.RuntimeConfig.ExperimentalGetTextUsingJs
            ? MapUsingJs<string>("return el.innerText").ToArray()
            : Map(el => el.Text).ToArray();

    /// <inheritdoc />
    public virtual int Count => FindAll().Count;

    /// <inheritdoc />
    public virtual bool IsDisplayed => TryExecuteOnElement(el => el.Displayed);

    /// <inheritdoc />
    public virtual bool Exists => TryFind() != null;

    /// <inheritdoc />
    public virtual string TagName => FindWebElement().TagName;

    /// <inheritdoc />
    public virtual string InnerHtml => GetAttribute("innerHTML");

    /// <inheritdoc />
    public virtual string OuterHtml => GetAttribute("outerHTML");

    /// <inheritdoc />
    public virtual string Value => GetAttribute("value");

    /// <inheritdoc />
    public string DomId =>
        FindWebElement().ReadField<string>("elementId")
        ?? throw new("Fatal error: field elementId is null"));

    /// <inheritdoc />
    public virtual bool HasClass(string cssClass) => CallAction("HasClass", () => {
        return GetAttribute("class").Split(" ").Contains(cssClass);
    });

    /// <inheritdoc />
    public virtual string GetCssValue(string cssPropertyName) => CallAction("GetCssValue", () => {
        return FindWebElement().GetCssValue(cssPropertyName);
    });

    /// <inheritdoc />
    public virtual string GetAttribute(string name) => CallAction("GetAttribute", () => {
        return FindWebElement().GetAttribute(name);
    });

    /// <inheritdoc />
    public virtual bool HasAttribute(string name) => CallAction("HasAttribute", () => {
        return FindWebElement().GetAttribute(name) != null;
    });

    /// <inheritdoc />
    public virtual ILocator Click() => CallAction("Click", () => {
        HighlightIfEnabled();
        FindWebElement().Click();
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator SendKeys(string text) => CallAction("SendKeys", () => {
        HighlightIfEnabled();
        FindWebElement().SendKeys(text);
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator SetValue(string text) => CallAction("SetValue", () => {
        var found = FindWebElement();
        var tag = found.TagName;
        if (tag != "input" && tag != "textarea")
            throw new NotImplementedException($"SetValue doesn't handle {tag}. Handle only input / textarea.");

        var el = found;
        HighlightIfEnabled();
        el.Clear();
        el.SendKeys(text);
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator Highlight(string cssColor = "red") => CallAction("Highlight", () => {
        ExecuteJs($"el.style.border = '2px solid {cssColor}'");
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator Hover() => CallAction("Hover", () => {
        HighlightIfEnabled();
        new Actions(GetDriver()).MoveToElement(FindWebElement()).Perform();
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator Focus() => CallAction("Focus", () => {
        HighlightIfEnabled();
        ExecuteJs("el.focus()");
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator Clear() => CallAction("Clear", () => {
        HighlightIfEnabled();
        FindWebElement().Clear();
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator ScrollIntoView() => CallAction("ScrollIntoView", () => {
        return ExecuteJs("el.scrollIntoView({behavior: 'smooth'})");
    });

    /// <inheritdoc />
    public virtual ILocator DragTo(ILocator target) => CallAction("DragTo", () => {
        HighlightIfEnabled();
        IWebElement targetEl = target.FindWebElement();
        ExecuteJs(ReadJsFile("dragAndDrop.js") + "executeDragAndDrop(el, arguments[0])", targetEl);

        return this;
// Selenium native implementation (causes problems)
// new Actions(getDriver()).dragAndDrop(find(), targetEl).perform();
    });

    /// <inheritdoc />
    public virtual ILocator ExecuteJs(string jsCode, params object[] jsArgs) => CallAction("ExecuteJs", () => {
        var jsDriver = (IJavaScriptExecutor)GetDriver();

        jsArgs = new List<object>(jsArgs) { FindWebElement() }.ToArray();
        jsDriver.ExecuteScript($"const el = arguments[{jsArgs.Length - 1}];{jsCode}", jsArgs);
        return this;
    });

    /// <inheritdoc />
    public virtual TReturnType ExecuteJs<TReturnType>(string jsCode, params object[] jsArgs) => CallAction("ExecuteJs", () => {
            var jsDriver = (IJavaScriptExecutor)GetDriver();

            // add return statement for trivial scripts if it's missing e.g. "el.value" => "return el.value"
            if (!Regex.IsMatch(jsCode, @"(\n|;|return|}|{)")) {
                jsCode = "return " + jsCode;
            }

            jsArgs = new List<object>(jsArgs) { FindWebElement() }.ToArray();
            var result = jsDriver.ExecuteScript($"const el = arguments[{jsArgs.Length - 1}];{jsCode}", jsArgs);
            return (TReturnType)result;
        });

    /// <inheritdoc />
    public virtual ILocator WaitFor(int? timeoutMs = null, bool throwIfFail = true) => CallAction<ILocator>("WaitFor", () => {
        try {
            return Waiter.WaitFor("Element exist.", Find, timeoutMs);
        } catch (Exception) {
            if (throwIfFail) throw;
            return this;
        }
    });

    /// <inheritdoc />
    public virtual ILocator WaitForAppear(int? timeoutMs = null, bool throwIfFail = true) => CallAction("WaitForAppear", () => {
        try {
            Waiter.WaitFor($"Element appear: \n{CssOrXpath}", () => Find().IsDisplayed, timeoutMs);
        } catch (Exception) {
            if (throwIfFail) throw;
        }

        return this;
    });

    /// <inheritdoc />
    public virtual ILocator WaitForDisappear(int? timeoutMs = null) => CallAction("WaitForDisappear", () => {
        Waiter.WaitFor($"Element disappear: \n{CssOrXpath}",
            () => !IsDisplayed,
            timeoutMs);
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator WaitForReload(Action? actionThatTriggerReload = null, int? timeoutMs = null) => CallAction("WaitForReload", () => {
        var beforeDomId = DomId;
        if(actionThatTriggerReload != null) actionThatTriggerReload.Invoke();
        Waiter.WaitFor("Reload element: " + CssOrXpath, () => DomId != beforeDomId, timeoutMs);
        return this;
    });

    /// <summary> Map every found element using JavaScript code.</summary>
    /// <param name="jsMappingCode"> js code where current element is defined as: 'el'</param>
    /// <typeparam name="T">Return type of js code</typeparam>
    /// <returns>List of all mapped elements</returns>
    public virtual IList<T> MapUsingJs<T>(string jsMappingCode) => CallAction("ExecuteJs", () =>  {
        return JsFinder.FindAllAndMap<T>(jsMappingCode);
    });

    /// <inheritdoc />
    public virtual List<T> Map<T>(Func<IFoundLocator, T> func)  => CallAction("ExecuteJs", () => {
        var elements = FindAll().ToList();
        var results = new List<T>();

        for (int i = 0; i < elements.Count; i++) {
            var element = elements[i];
            try {
                results.Add(func.Invoke(element));
            } catch (Exception e) {
                var elementHtml = element.OuterHtml.TrimToMaxLength(100);
                throw new Exception($"Mapping failed during processing element [{i}/{elements.Count}]: " + elementHtml,
                    e);
            }
        }

        return results;
    });

    private T CallAction<T>(string actionName, Func<T> func) {
        //beforeActionhook
        return func.Invoke();
        //afterActionHook
    }

    /// <inheritdoc />
    public override string ToString() => Description;

    private string ReadJsFile(string jsFileName) {
        return File.ReadAllText("EasyDriver\\Js\\" + jsFileName);
    }

    private WebElementFinder WebElementFinder => new(GetDriver(), this);
    private JsFinder JsFinder => new(GetDriver(), this);

    private void HighlightIfEnabled() {
        if (Configuration.RuntimeConfig.HighlightActions) {
            var found = TryFind();
            if (found != null) found.Highlight();
        }
    }

    private T? TryExecuteOnElement<T>(Func<IWebElement, T> func) {
        try {
            return func.Invoke(FindWebElement());
        } catch (LocatorException) {
            return default;
        } catch (StaleElementReferenceException) {
            return default;
        }
    }
}