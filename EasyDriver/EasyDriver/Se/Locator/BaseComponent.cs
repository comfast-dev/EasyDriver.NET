using System.Collections.ObjectModel;
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
/// Base class for every component.
/// Example implementation:
/// <see cref="LinkByHref"/>
/// </summary>
public abstract class BaseComponent : ILocator {
    /// <summary>
    /// Create new Child selector based on this as parent.
    /// </summary>
    public ILocator _S(string cssOrXpath) {
        return new SimpleLocator(Selector + WebElementFinder.SelectorSeparator + cssOrXpath);
    }

    /// <summary>
    /// Css / Xpath selector
    /// </summary>
    public abstract string Selector { get; }

    /// <summary>
    /// Description for this Component / Locator
    /// </summary>
    public abstract string? Description { get; }

    /// <inheritdoc />
    public virtual IWebElement DoFind() => Finder.Find();

    /// <inheritdoc />
    public virtual IFoundLocator Find() => new FoundLocator(Selector, Description, DoFind());

    /// <inheritdoc />
    public virtual IReadOnlyCollection<IFoundLocator> FindAll() =>
        DoFindAll()
            .Select(webEl => new FoundLocator(Selector, Description, webEl)).ToList();

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
        var all = DoFindAll();
        if (all.Count < number)
            throw new Exception($"Not found element #{number}. There are {all.Count} matched by:\n{Selector}");

        return new FoundLocator(Selector, Description, all[number - 1]);
    }

    /// <inheritdoc />
    public virtual string Text => DoFind().Text;

    /// <inheritdoc />
    public virtual string[] Texts => Map(el => el.Text).ToArray();

    /// <inheritdoc />
    public virtual int Count => FindAll().Count;

    /// <inheritdoc />
    public virtual bool IsDisplayed => Exists && DoFind().Displayed;

    /// <inheritdoc />
    public virtual bool Exists => TryFind() != null;

    /// <inheritdoc />
    public virtual string TagName => DoFind().TagName;

    /// <inheritdoc />
    public virtual string InnerHtml => GetAttribute("innerHTML");

    /// <inheritdoc />
    public virtual string OuterHtml => GetAttribute("outerHTML");

    /// <inheritdoc />
    public virtual string Value => GetAttribute("value");

    /// <inheritdoc />
    public virtual bool HasClass(string cssClass) => GetAttribute("class").Split(" ").Contains(cssClass);

    /// <inheritdoc />
    public virtual ILocator Click() {
        HighlightIfEnabled();
        DoFind().Click();
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator SendKeys(string text) {
        HighlightIfEnabled();
        DoFind().SendKeys(text);
        return this;
    }

    /// <inheritdoc />
    public virtual ILocator SetValue(string text) {
        //todo use more logic, handle Inputs, Selects, Checkboxes
        // new AnyInput(DoFind()).SetValue(text)

        var found = Find();
        var tag = found.TagName;
        if (tag != "input" && tag != "textarea")
            throw new NotImplementedException($"SetValue doesn't handle {tag}. Handle only input / textarea.");

        var el = found.FoundElement;
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
        new Actions(GetDriver()).MoveToElement(DoFind()).Perform();
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
        DoFind().Clear();
        return this;
    }

    /// <inheritdoc />
    public virtual string GetCssValue(string propertyName) {
        return DoFind().GetCssValue(propertyName);
    }

    /// <inheritdoc />
    public virtual ILocator DragTo(ILocator target) {
        HighlightIfEnabled();
        IWebElement targetEl = target.DoFind();
        ExecuteJs(ReadJsFile("dragAndDrop.js") + "executeDragAndDrop(el, arguments[0])", targetEl);

        return this;
// Selenium native implementation (causes problems)
// new Actions(getDriver()).dragAndDrop(find(), targetEl).perform();
    }

    /// <inheritdoc />
    public ILocator ExecuteJs(string jsCode, params object[] jsArgs) {
        var jsDriver = (IJavaScriptExecutor)GetDriver();

        jsArgs = FindAndAddToArgs(jsArgs);
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

        jsArgs = FindAndAddToArgs(jsArgs);
        var result = jsDriver.ExecuteScript($"const el = arguments[{jsArgs.Length - 1}];{jsCode}", jsArgs);
        return (TReturnType)result;
    }

    private object[] FindAndAddToArgs(object[] jsArgs) => new List<object>(jsArgs) { DoFind() }.ToArray();

    /// <inheritdoc />
    public virtual string GetAttribute(string name) => DoFind().GetAttribute(name);

    /// <inheritdoc />
    public bool HasAttribute(string name) => GetAttribute(name) != null;

    /// <inheritdoc />
    public virtual ILocator WaitFor(int? timeoutMs = null, bool throwIfFail = true) {
        try {
            WaitUtils.WaitFor(() => IsDisplayed, "Element displayed.", timeoutMs);
        } catch (Exception) {
            if (throwIfFail) throw;
        }

        return this;
    }

    /// <inheritdoc />
    public virtual ILocator WaitForDisappear(int? timeoutMs = null) {
        WaitUtils.WaitFor(() => !IsDisplayed, "Element disappear.", timeoutMs);
        return this;
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
                var elementHtml = element.GetAttribute("outerHTML").LimitString(100);
                throw new Exception($"Mapping failed during processing element [{i}/{elements.Count}]: " + elementHtml,
                    e);
            }
        }

        return results;
    }

    /// <inheritdoc />
    public override string ToString() => Description ?? "??";

    protected virtual ReadOnlyCollection<IWebElement> DoFindAll() => Finder.FindAll();

    private string ReadJsFile(string jsFileName) {
        return new StreamReader("EasyDriver\\Js\\" + jsFileName).ReadToEnd();
    }

    private WebElementFinder Finder => new(DriverApi.GetDriver(), Selector);

    private void HighlightIfEnabled() {
        if (Configuration.LocatorConfig.HighlightActions) {
            var found = TryFind();
            if (found != null) found.Highlight();
        }
    }
}