using OpenQA.Selenium;

namespace Comfast.EasyDriver;

public interface ILocator {

    public string Text { get; }
    public string[] Texts { get; }
    public int Count { get; }
    public bool IsDisplayed { get; }
    public bool Exists { get; }
    public string TagName { get; }
    public string InnerHtml { get; }
    public string OuterHtml { get; }

    public string Value { get; }
    // public string CssValue { get; }
    // S(string cssOrXpath)

    public bool HasClass(string cssClass);
    public ILocator Click();
    public ILocator SetValue(string text);
    public ILocator Highlight(string cssColor = "red");
    public string GetAttribute(string name);
    public ILocator WaitFor(int? timeoutMs = null, bool throwIfFail = true);
    public ILocator WaitForDisappear(int? timeoutMs = null);
    public ILocator SendKeys(string text);

    // public ILocator Tap();
    // public ILocator Focus();
    // public ILocator Hover();
    // public ILocator Clear();
    // public ILocator Type();
    // public ILocator DragTo(ILocator target);
    // public ILocator SetChecked(bool checked);

    public IFoundLocator Find();
    public IFoundLocator? TryFind();
    public IReadOnlyCollection<IFoundLocator> FindAll();
    public IWebElement DoFind();

    public List<T> Map<T>(Func<IFoundLocator, T> func);
    // public ILocator ForEach(Action<ILocator> func);
    // public ILocator GetNth(int number);
}