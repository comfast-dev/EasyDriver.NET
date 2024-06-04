using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Comfast.Commons.Utils;
using Comfast.EasyDriver.Se.Finder;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Se.Locator;

public abstract class BaseComponent : ILocator {
    public abstract SelectorChain Chain { get; }
    public abstract string? Description { get; }

    private WebElementFinder Finder => new(GetDriver(), Chain);

    protected virtual IWebDriver GetDriver() => Configuration.GetDriver();

    public virtual IWebElement DoFind() => Finder.Find();

    protected virtual ReadOnlyCollection<IWebElement> DoFindAll() => Finder.FindAll();

    public virtual IFoundLocator Find() => new FoundSeleniumLocator(Chain, Description, DoFind());

    public virtual IReadOnlyCollection<IFoundLocator> FindAll() => DoFindAll()
        .Select(webEl => new FoundSeleniumLocator(Chain, Description, webEl)).ToList();

    public virtual IFoundLocator? TryFind() {
        try {
            return Find();
        } catch (Exception) {
            return null;
        }
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
        return this;
    }

    public virtual ILocator SendKeys(string text) {
        DoFind().SendKeys(text);
        return this;
    }

    public virtual ILocator SetValue(string text) {
        //todo use more logic, handle Inputs, Selects, Checkboxes
        // new AnyInput(DoFind()).SetValue(text)

        var found = Find();
        var tag = found.TagName;
        if (tag != "input" && tag != "textarea") throw new NotImplementedException($"SetValue doesn't handle {tag}. Handle only input / textarea.");

        var el = found.FoundElement;
        el.Clear();
        el.SendKeys(text);
        return this;
    }

    public virtual ILocator Highlight(string cssColor = "red") {
        ExecuteJs($"el.style.border = '2px solid {cssColor}'");
        return this;
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