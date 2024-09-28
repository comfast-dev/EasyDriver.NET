using System.Diagnostics;
using System.Text.RegularExpressions;
using Comfast.Commons.Utils;
using Comfast.EasyDriver.Core.Errors;
using Comfast.EasyDriver.Core.Finder;
using Comfast.EasyDriver.Js;
using Comfast.EasyDriver.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Comfast.EasyDriver.Core.Locator;

/// <summary> Base class for component. </summary>
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
        return GetFinder().Find();
    });

    /// <inheritdoc />
    public virtual IList<IWebElement> FindWebElements() => CallAction("FindElements", () => {
        return WebElementFinder.FindAll();
    });

    private JsFinder JsFinder => new(GetDriver(), this);
    private WebElementFinder WebElementFinder => new(GetDriver(), this);
    private IFinder<IWebElement> GetFinder() {
        return Configuration.RuntimeConfig.ExperimentalJsFinder
            ? JsFinder : WebElementFinder;
    }

    /// <inheritdoc />
    public virtual IFoundLocator Find() => CallAction("Find", () => {
        return new FoundLocator(CssOrXpath, Description, FindWebElement());
    });

    /// <inheritdoc />
    public virtual IList<IFoundLocator> FindAll() => CallAction("FindAll", () => {
        return FindWebElements()
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
        var all = FindWebElements();
        if (all.Count < number)
            throw new Exception($"Not found element #{number}. There are {all.Count} matched by:\n{CssOrXpath}");

        return new FoundLocator(CssOrXpath, Description, all[number - 1]);
    });

    /// <inheritdoc />
    public virtual string Text => CallPropertyAction("Text", () => ExecuteJs<string>("return el.innerText"));

    /// <inheritdoc />
    public virtual string[] Texts => CallPropertyAction("Texts", () =>
        Configuration.RuntimeConfig.ExperimentalJsActions
            ? MapUsingJs<string>("return el.innerText").ToArray()
            : Map(el => el.Text).ToArray());

    /// <inheritdoc />
    public virtual int Count => CallPropertyAction("Count", () => FindAll().Count);

    /// <inheritdoc />
    public virtual bool IsDisplayed => CallPropertyAction("IsDisplayed", () => TryToExecuteOnElement(el => el.Displayed));

    /// <inheritdoc />
    public virtual bool Exists => CallPropertyAction("Exists", () => TryFind() != null);

    /// <inheritdoc />
    public virtual string TagName => CallPropertyAction("Exists", () => FindWebElement().TagName);

    /// <inheritdoc />
    public virtual string InnerHtml => CallPropertyAction("InnerHtml", () => GetAttribute("innerHTML"));

    /// <inheritdoc />
    public virtual string OuterHtml => CallPropertyAction("OuterHtml", () => GetAttribute("outerHTML"));

    /// <inheritdoc />
    public virtual string Value => CallPropertyAction("Value", () => GetAttribute("value"));

    /// <inheritdoc />
    public string DomId => CallPropertyAction("DomId", () =>
        FindWebElement().ReadField<string>("elementId")
        ?? throw new("Fatal error: field elementId is null"));

    /// <inheritdoc />
    public virtual bool HasCssClass(string cssClass) => CallAction("HasClass", () => {
        return GetAttribute("class").Split(" ").Contains(cssClass);
    });

    /// <inheritdoc />
    public virtual string GetCssValue(string cssPropertyName) => CallAction("GetCssValue", () => {
        // if (Configuration.RuntimeConfig.ExperimentalJsActions) {
        //     return JsFinder.FindAndCallJs<string>($"return el.style['{cssPropertyName}']");
        // }
        return FindWebElement().GetCssValue(cssPropertyName);
    });

    /// <inheritdoc />
    public virtual string? GetAttribute(string name) => CallAction("GetAttribute", () => {
        if (Configuration.RuntimeConfig.ExperimentalJsActions) {
            var value = JsFinder.FindAndExecuteJs<object>($"return (el.{name} || el.getAttribute('{name}'))");
            return value?.ToString();
        }
        return FindWebElement().GetAttribute(name);
    });

    /// <inheritdoc />
    public virtual bool HasAttribute(string name) => CallAction("HasAttribute", () => {
        return GetAttribute(name) != null;
    });

    /// <inheritdoc />
    public virtual ILocator Click() => CallAction("Click", () => {
        HandleHighlight();
        FindWebElement().Click();
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator SendKeys(string text) => CallAction("SendKeys", () => {
        HandleHighlight();
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
        HandleHighlight();
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
        HandleHighlight();
        new Actions(GetDriver()).MoveToElement(FindWebElement()).Perform();
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator Focus() => CallAction("Focus", () => {
        HandleHighlight();
        ExecuteJs("el.focus()");
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator UnFocus() => CallAction("UnFocus", () => {
        ExecuteJs("el.blur()");
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator Clear() => CallAction("Clear", () => {
        HandleHighlight();
        FindWebElement().Clear();
        return this;
    });

    /// <inheritdoc />
    public virtual ILocator ScrollIntoView() => CallAction("ScrollIntoView", () => {
        return ExecuteJs("el.scrollIntoView({behavior: 'smooth'})");
    });

    /// <inheritdoc />
    public virtual ILocator DragTo(ILocator target) => CallAction("DragTo", () => {
        HandleHighlight();
        IWebElement targetEl = target.FindWebElement();
        ExecuteJs(JsFiles.ReadJsFile("dragAndDrop.js") + "executeDragAndDrop(el, arguments[0])", targetEl);

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
    public virtual ILocator WaitFor(int? timeoutMs = null, bool throwIfFail = true) => CallAction("WaitFor", () => {
        try {
            return Waiter.WaitFor("Element exist.", Find, timeoutMs);
        } catch (Exception) {
            if (throwIfFail) throw;
            return (ILocator)this;
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
    public virtual IList<T> MapUsingJs<T>(string jsMappingCode) => CallAction("MapUsingJs",
        () => new JsFinder(GetDriver(), this).FindAllAndMapUsingJs<T>(jsMappingCode));

    /// <inheritdoc />
    public virtual List<T> Map<T>(Func<IFoundLocator, T> func)  => CallAction("Map", () => {
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

    /// <inheritdoc />
    public override string ToString() => Description;

    private void HandleHighlight() {
        if (Configuration.RuntimeConfig.HighlightActions) {
            var found = TryFind();
            if (found != null) found.Highlight();
        }
    }

    private T? TryToExecuteOnElement<T>(Func<IWebElement, T> func) {
        try {
            return func.Invoke(FindWebElement());
        } catch (LocatorException) {
            return default;
        } catch (StaleElementReferenceException) {
            return default;
        }
    }



    class BeforeAction {
        private string name;

    }

    class ActionHooks {
        void onActionStart(string name) {

        }
    }

    private T CallPropertyAction<T>(string propName, Func<T> func) => CallAction("Get" + propName, func);
    private T CallAction<T>(string actionName, Func<T> func) {
        T? actionResult = default(T);
        var sw = new Stopwatch();
        sw.Start();
        try {
            //onActionStart
            actionResult = func.Invoke();
            //onActionEnd
        } catch (Exception) {
            //onerror
        } finally {
            sw.Stop();
        }

        return actionResult!;
    }


}