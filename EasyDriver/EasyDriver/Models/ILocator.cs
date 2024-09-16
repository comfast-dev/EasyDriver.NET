using Comfast.EasyDriver.Se.Finder;
using OpenQA.Selenium;

namespace Comfast.EasyDriver.Models;

/// <summary> Main locator interface</summary>
public interface ILocator {
    /// <summary>Alias to <see cref="SubLocator"/></summary>
    public ILocator _S(string cssOrXpath, string? description = null);

    /// <summary> Creates new SubLocator</summary>
    /// <param name="cssOrXpath">CSS / XPATH selector</param>
    /// <param name="description">used in error messages and logs</param>
    /// <returns></returns>
    public ILocator SubLocator(string cssOrXpath, string? description = null);

    /// <summary> Element selector</summary>
    public string Selector { get; }

    /// <summary> Element description (as metadata used for error messages and logs)</summary>
    public string Description { get; }

    /// <summary> Get SelectorChain component</summary>
    public SelectorChain SelectorChain => new(Selector);

    /// <summary> Element text</summary>
    public string Text { get; }

    /// <summary> All element texts matched by selector</summary>
    public string[] Texts { get; }

    /// <summary> Count of elements matched by selector</summary>
    public int Count { get; }

    /// <summary> true if element exist in DOM and displayed on screen. Does not require to be in current viewport.</summary>
    public bool IsDisplayed { get; }

    /// <summary> true if element exist in DOM. Can be hidden.</summary>
    public bool Exists { get; }

    /// <summary> Element tag name e.g. "form"</summary>
    public string TagName { get; }

    /// <summary> Inner HTML code of element. Equivalent to JS code: element.innerHTML</summary>
    public string InnerHtml { get; }

    /// <summary> Inner HTML code of element. Equivalent to JS code: element.outerHTML</summary>
    public string OuterHtml { get; }

    /// <summary> Element value property. Equivalent to JS code: element.value</summary>
    public string Value { get; }

    /// <summary> Internal element DOM Id. Unique, changes during element refresh.</summary>
    public string DomId { get; }

    // public string CssValue { get; }

    /// <summary>
    /// Check for CSS class. e.g. <br/>
    /// <input class="myclass disabled" />
    /// <code>
    /// HasClass("myclass") // return true
    /// HasClass("disabled") // return true
    /// HasClass("lol") // return false
    /// HasClass("myclass disabled") // return false - it doesn't check 'class' property, but CSS class
    ///</code>
    /// </summary>
    public bool HasClass(string cssClass);

    /// <summary> Gets value of CSS Property. Native WebDriver method: GetCssValue.</summary>
    public string GetCssValue(string cssPropertyName);

    /// <summary> Click the element</summary>
    public ILocator Click();

    /// <summary> SetValue to standard Form elements like input, textarea, select, checkbox</summary>
    public ILocator SetValue(string text);

    /// <summary> Set element border with given color to focus attention</summary>
    /// <param name="cssColor"></param>
    /// <returns></returns>
    public ILocator Highlight(string cssColor = "red");

    /// <summary> Get element attribute value, null if not found.</summary>
    public string? GetAttribute(string name);

    /// <summary> true - if attribute is present</summary>
    public bool HasAttribute(string attribute);

    /// <summary> Wait for element. Don't check visibility.</summary>
    /// <param name="timeoutMs">custom timeout</param>
    /// <param name="throwIfFail">if false - function doesn't throw Exception and stop the program</param>
    public ILocator WaitFor(int? timeoutMs = null, bool throwIfFail = true);

    /// <summary> Wait for element to appear.</summary>
    /// <param name="timeoutMs">custom timeout</param>
    /// <param name="throwIfFail">if false - function doesn't throw Exception and stop the program</param>
    public ILocator WaitForAppear(int? timeoutMs = null, bool throwIfFail = true);

    /// <summary> Wait for element to disappear from DOM</summary>
    /// <param name="timeoutMs">custom timeout</param>
    public ILocator WaitForDisappear(int? timeoutMs = null);

    /// <summary> Executes action and wait until given locator reload</summary>
    /// <param name="actionThatTriggerReload">OPTIONAL: Action that should trigger reload</param>
    /// <param name="timeoutMs">custom timeout</param>
    public ILocator WaitForReload(Action? actionThatTriggerReload = null, int? timeoutMs = null);

    /// <summary> Does keyboard input</summary>
    public ILocator SendKeys(string text);

    // public ILocator Tap();

    /// <summary> Focus on element e.g. set cursor on it.</summary>
    public ILocator Focus();

    /// <summary> Move mouse over element.</summary>
    public ILocator Hover();

    /// <summary> Clear element value</summary>
    public ILocator Clear();

    /// <summary> Scrolls to element</summary>
    public ILocator ScrollIntoView();

    /// <summary> Drag and Drop this element over another element</summary>
    /// <param name="target">target locator</param>
    public ILocator DragTo(ILocator target);

    // public ILocator SetChecked(bool checked);

    /// <summary> Find ane return object with fixed DOM element for further actions.</summary>
    public IFoundLocator Find();

    /// <summary> Tries find element, doesn't throw Exception.</summary>
    public IFoundLocator? TryFind();

    /// <summary>
    /// Find all elements matched by locator.<br/>
    /// Return found locators with fixed found DOM element.
    /// </summary>
    public IList<IFoundLocator> FindAll();

    /// <summary> Finds Selenium WebElement.</summary>
    public IWebElement FindElement();

    /// <summary> Finds all elements matched by locator</summary>
    public IList<IWebElement> FindElements();

    /// <summary>
    /// Perform action on all found elements.<br/>
    /// It is equivalent to:<br/>
    /// - C# LINQ .Select() method<br/>
    /// - JAVA / JavaScript .map() method.<br/>
    /// </summary>
    /// <example><code>
    /// string[] allLinksInTable = S("table.myTable a").Map(el => el.GetAttribute("href")).ToArray();
    /// </code></example>
    public List<T> Map<T>(Func<IFoundLocator, T> func);

    /// <summary>
    /// Map every found element using JavaScript code.
    /// Works faster than standard mapping. Calls one Selenium request.
    /// <example>
    /// var htmls = Locator("td").MapUsingJs("return el.innerHTML");
    /// </example>
    /// </summary>
    /// <param name="jsMappingCode"> js code where current element is defined as: 'el'</param>
    /// <typeparam name="T">Return type of js code</typeparam>
    /// <returns>List of all mapped elements</returns>
    public IList<T> MapUsingJs<T>(string jsMappingCode);

    /// <summary>
    /// Find all elements and return nth found DOM element.<br/>
    /// where first is 1
    /// </summary>
    public IFoundLocator Nth(int number);

    /// <summary>
    /// Execute JavaScript code, where current element is "el" variable<br/>
    /// Examples:<code>
    /// S("table").ExecuteJs("el.style.padding = '0px'");<br/>
    ///
    /// //with args:
    /// var myFontSize = 15.5;
    /// S("p").ExecuteJs("el.style.fontSize = arguments[0]", myFontSize);</code>
    /// </summary>
    /// <param name="jsCode">javascript</param>
    /// <param name="jsArgs">passed as arguments[] array</param>
    public ILocator ExecuteJs(string jsCode, params object[] jsArgs);

    /// <summary>
    /// Examples:<code>
    /// var href = S("a").ExecuteJs&lt;string&gt;("return el.href");</code>
    /// </summary>
    /// <param name="jsCode">javascript</param>
    /// <param name="jsArgs">passed as arguments[] array</param>
    /// <typeparam name="TReturnType"></typeparam>
    /// <returns>Javascript result</returns>
    public TReturnType ExecuteJs<TReturnType>(string jsCode, params object[] jsArgs);
}