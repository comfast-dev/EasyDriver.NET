using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Comfast.Commons.Utils;
using Comfast.EasyDriver.Models;
using Comfast.EasyDriver.Se.Finder;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Comfast.EasyDriver.Se.Locator;

/// <summary>
/// Base class for every component.
/// Example implementation:
/// <see cref="LinkComponent"/>
/// </summary>
public abstract class BaseComponent : ILocator {
    public abstract string Selector { get; }
    public abstract string? Description { get; }

    private WebElementFinder Finder => new(GetDriver(), Selector);

    protected virtual IWebDriver GetDriver() => Configuration.GetDriver();

    public virtual IWebElement DoFind() => Finder.Find();

    protected virtual ReadOnlyCollection<IWebElement> DoFindAll() => Finder.FindAll();

    public virtual IFoundLocator Find() => new FoundLocator(Selector, Description, DoFind());

    public virtual IReadOnlyCollection<IFoundLocator> FindAll() => DoFindAll()
        .Select(webEl => new FoundLocator(Selector, Description, webEl)).ToList();

    public virtual IFoundLocator? TryFind() {
        try {
            return Find();
        } catch (Exception) {
            return null;
        }
    }

    public virtual IFoundLocator Nth(int number) {
        var all = DoFindAll();
        if (all.Count < number)
            throw new Exception($"Not found element #{number}. There are {all.Count} matched by:\n{Selector}");

        return new FoundLocator(Selector, Description, all[number - 1]);
    }

    public virtual string Text => DoFind().Text;
    public virtual string[] Texts => Map(el => el.Text).ToArray();
    public virtual int Count => FindAll().Count;
    public virtual bool IsDisplayed => Exists && DoFind().Displayed;
    public virtual bool Exists => TryFind() != null;
    public virtual string TagName => DoFind().TagName;
    public virtual string InnerHtml => GetAttribute("innerHTML");
    public virtual string OuterHtml => GetAttribute("outerHTML");
    public virtual string Value => GetAttribute("value");

    public virtual bool HasClass(string cssClass) => GetAttribute("class").Split(" ").Contains(cssClass);

    public virtual ILocator Click() {
        DoFind().Click();
        HighlightIfEnabled();
        return this;
    }

    public virtual ILocator SendKeys(string text) {
        DoFind().SendKeys(text);
        HighlightIfEnabled();
        return this;
    }

    public virtual ILocator SetValue(string text) {
        //todo use more logic, handle Inputs, Selects, Checkboxes
        // new AnyInput(DoFind()).SetValue(text)

        var found = Find();
        var tag = found.TagName;
        if (tag != "input" && tag != "textarea")
            throw new NotImplementedException($"SetValue doesn't handle {tag}. Handle only input / textarea.");

        var el = found.FoundElement;
        el.Clear();
        el.SendKeys(text);
        HighlightIfEnabled();
        return this;
    }

    public virtual ILocator Highlight(string cssColor = "red") {
        ExecuteJs($"el.style.border = '2px solid {cssColor}'");
        return this;
    }

    private void HighlightIfEnabled() {
        if (Configuration.LocatorConfig.HighlightActions) {
            var found = TryFind();
            if (found != null) found.Highlight();
        }
    }

    public virtual ILocator Hover() {
        new Actions(GetDriver()).MoveToElement(DoFind()).Perform();
        HighlightIfEnabled();
        return this;
    }

    public virtual ILocator Focus() {
        ExecuteJs("el.focus()");
        HighlightIfEnabled();
        return this;
    }

    public virtual ILocator Clear() {
        DoFind().Clear();
        HighlightIfEnabled();
        return this;
    }

    public virtual string GetCssValue(string propertyName) {
        return DoFind().GetCssValue(propertyName);
    }

    public virtual ILocator DragTo(ILocator target) {
            IWebElement targetEl = target.DoFind();
            ExecuteJs(ReadJsFile("dragAndDrop.js") + "executeDragAndDrop(el, arguments[0])", targetEl);

            HighlightIfEnabled();
            return this;
// Selenium native implementation (causes problems)
// new Actions(getDriver()).dragAndDrop(find(), targetEl).perform();
    }

    private string ReadJsFile(string jsFileName) {
        return new StreamReader("EasyDriver\\Js\\" + jsFileName).ReadToEnd();
    }

    public ILocator ExecuteJs(string jsCode, params object[] jsArgs) {
        var jsDriver = (IJavaScriptExecutor)GetDriver();

        jsArgs = FindAndAddToArgs(jsArgs);
        jsDriver.ExecuteScript($"const el = arguments[{jsArgs.Length - 1}];{jsCode}", jsArgs);
        return this;
    }

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

    public virtual string GetAttribute(string name) => DoFind().GetAttribute(name);

    public bool HasAttribute(string name) => GetAttribute(name) != null;

    public virtual ILocator WaitFor(int? timeoutMs = null, bool throwIfFail = true) {
        WaitUtils.WaitFor(() => IsDisplayed, "Element displayed.", timeoutMs);
        return this;
    }

    public virtual ILocator WaitForDisappear(int? timeoutMs = null) {
        WaitUtils.WaitFor(() => !IsDisplayed, "Element disappear.", timeoutMs);
        return this;
    }

    public virtual List<T> Map<T>(Func<IFoundLocator, T> func) {
        var elements = FindAll().ToList();
        var results = new List<T>();

        for (int i = 0; i < elements.Count; i++) {
            var element = elements[i];
            try {
                results.Add(func.Invoke(element));
            } catch (Exception e) {
                var elementHtml = element.GetAttribute("outerHTML").LimitString(100);
                throw new Exception($"Mapping failed during processing element [{i}/{elements.Count}]: " + elementHtml, e);
            }
        }

        return results;
    }

    public override string ToString() => Description ?? "??";
}