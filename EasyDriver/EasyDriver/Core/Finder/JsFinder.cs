using System.Collections.ObjectModel;
using Comfast.EasyDriver.Core.Errors;
using Comfast.EasyDriver.Core.Locator;
using Comfast.EasyDriver.Js;
using Comfast.EasyDriver.Models;
using OpenQA.Selenium;
using Exception = System.Exception;

namespace Comfast.EasyDriver.Core.Finder;

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
public class JsFinder : IFinder<IWebElement> {
    private static readonly string FinderJsCode = JsFiles.ReadJsFile("finder.js");

    private readonly IJavaScriptExecutor _jsDriver;
    private readonly string[] _selectorsArgs;
    private readonly ILocator _locator;

    public JsFinder(IWebDriver webDriver, string selector) : this(webDriver, new SimpleLocator(selector, "Element")) { }

    public JsFinder(IWebDriver webDriver, ILocator locator) {
        _jsDriver = (IJavaScriptExecutor)webDriver;
        _selectorsArgs = locator.SelectorChain.SelectorsArray;
        _locator = locator;
    }

    /// <summary> Find first matched element</summary>
    public IWebElement Find() {
        var result = _jsDriver.ExecuteScript(FinderJsCode + "return find(arguments)", _selectorsArgs);
        if (result is IWebElement foundElement) return foundElement;

        throw new LocatorNotFoundException(_locator, (int)(long)result);
    }

    /// <summary> Find all matched elements</summary>
    public IList<IWebElement> FindAll() {
        var result = _jsDriver.ExecuteScript(FinderJsCode + "return findAll(arguments)", _selectorsArgs);
        return ConvertJsCollectionResult<IWebElement>(result);
    }

    /// <summary> Find element and call JS on it. </summary>
    public T FindAndExecuteJs<T>(string jsCode) {
        var funcCode = $"\nfunction func(el) {{\n{jsCode}\n}}";
        var result = _jsDriver.ExecuteScript(FinderJsCode + funcCode + "return func(find(arguments))", _selectorsArgs);
        return (T)result;
    }

    /// <summary> Executes given JS Code on every result.</summary>
    /// <param name="jsCode">Executes this code on every element, where el is element variable</param>
    /// <typeparam name="T">return type of given function</typeparam>
    /// <returns>Array of results</returns>
    public IList<T> FindAllAndMapUsingJs<T>(string jsCode) {
        var funcCode = $"\nfunction func(el) {{\n{jsCode}\n}}";
        var result = _jsDriver.ExecuteScript(
            FinderJsCode + funcCode + "return Array.from(findAll(arguments)).map(func)", _selectorsArgs);
        return ConvertJsCollectionResult<T>(result);
    }

    private static IList<T> ConvertJsCollectionResult<T>(object result) {
        if (result == null) throw new("Internal Error: null response from JS");
        if (result is ReadOnlyCollection<T> resT) return resT;
        if (result is ReadOnlyCollection<object> resObj) return resObj.Select(el => (T)el).ToList();
        throw new Exception("Internal error: Invalid return from Javascript console. Actual: " + result.GetType().Name);
    }
}