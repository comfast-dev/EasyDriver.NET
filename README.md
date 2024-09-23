# EasyDriver
It is library on top of Selenium WebDriver that that solves common problems and let write short and concise code.
Main features:
- Manages browser / WebDriver instances: One thread => one browser
  - ready for parallel runs in any framework like XUnit, NUnit, MSTest
  - exposes static api for getting WebDriver instance: `DriverApi.GetDriver()`
  - Reconnects to already opened browser
- Expose universal API for CSS/XPATH locators `ILocator`
  - Configurable auto Wait
  - Extensive API for actions/
  - Short API `GetLocator("css or xpath selector").Click()`
  - Sub-selectors like `GetLocator("//table").SubLocator("td.selected")`

### Locators:
Main entrypoint is `EasyDriverApi `.
```csharp
using Comfast.EasyDriver.EasyDriverApi
```

Basic locator and Selenium elements
```csharp
ILocator MyButton = EasyDriverApi.GetLocator("css or xpath selector here");
MyButton.Click();
var text = MyButton.Text

// there are always option to get/use native selenium WebElements/WebDriver:
IWebDriver driver = EasyDriverApi.GetDriver();
IWebElement foundButton = MyButton.FindWebElement();
```

To get short locator syntax import statically EasyDriverApi
```
//if DriverApi imported statically it can be shorter:
using static Comfast.EasyDriver.EasyDriverApi;
GetLocator("#myButton").Click(); // by css
GetLocator("//button[@id='myButton']").Click() // by xpath
```




## Components concept
EasyDriver provides API to model parts of DOM into classes.
Let's analyse this example HTML of form:
```html
(...)
<form id="myForm">
    <input type="text" name="username" />
    <input type="text" name="password" />
    <button type="submit" />
</form>
```
It can be modelled as C# class in few lines:
```csharp
class MyForm : BaseComponent {
    //fields required to implement: CssOrXpath, Description
    public string CssOrXpath => "#myForm"
    public string Description => "My form";

    //these Sub-locators will be searched as children of parent: "#myForm"
    ILocator userInput = SubLocator("//input[@name='username']");
    ILocator passwordInput = SubLocator("//input[@name='password']");
    ILocator submitBtn = SubLocator("button[type=submit]");

    public Send(string user, string pass) {
        userInput.SetValue(user);
        passwordInput.SetValue(pass);
        submitBtn.Click();
    }
}
```
Where:
1. `CssOrXpath` is same selector string as can be passed to `GetLocator()` method
2. `Description` is required only as metadata used in Error messages/logs - don't important in code run
3. `MyForm` implements `ILocator` interface itself, so all methods like `this.Click()` can be called (`#myForm` will be clicked here.
4. Locator won't perform search if action like Click, SetValue) is not called, so class can be created at any point of program.


## Configuration
Here is example `EasyDriverConfig.json` that need to be placed in project root path:
```json
{
  "BrowserConfig": {
    "BrowserPath": "c:\\some\\chromium\\path\\chrome.exe",
    "DriverPath": "c:\\some\\chromedriver\\path\\chromedriver.exe",
    "Reconnect": false,
    "AutoClose": true,
    "Headless": false
  },
  "RuntimeConfig": {
    "TimeoutMs": 20000
  }
}
```
Where:
- **BrowserPath** - path to browser executable, e.g. chrome.exe, firefox.exe, etc.
- **DriverPath** - path to chromedriver.exe / geckodriver.exe / edgedriver.exe etc.
- **Reconnect** - should try to reuse same browser between runs.
- **AutoClose** - should browser be closed after test run - if you use reconnect, set it to false
- **Headless** - makes tests faster, but don't show browser UI
- **TimeoutMs** - default timeout for wait methods (where not specified in call code)
