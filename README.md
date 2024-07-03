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
  - Short API `S("css or xpath selector")`
  - Sub-selectors like `S("//table")._s("td.selected")`

### Locators:
Main entrypoint is `DriverApi `. 
```csharp
using Comfast.EasyDriver.DriverApi
```

Basic locator and Selenium elements
```
ILocator MyButton = DriverApi.S("css or xpath selector here");
MyButton.Click();
var text = MyButton.Text

// there are always option to get native selenium WebElements/WebDriver: 
IWebDriver driver = DriverApi.GetDriver();
IWebElement foundButton = MyButton.DoFind();
```

Import statically DriverApi to have short locator syntax
```
//if DriverApi imported statically it can be shorter:
using static Comfast.EasyDriver.DriverApi;
S("#myButton").Click(); // by css
S("//button[@id='myButton']").Click() // by xpath
```

Creatinc components like `MyForm` here
```
// locators won't perform search if you don't call action
var btn = S("#myButton"); //this code will not call browser
btn.Click(); //this code will find element by CSS selector and click

//so, locator'sare cheap and can be used as class fields without problem
class MyForm {
    ILocator nameInput = S("//input[@name='name']");
    ILocator submitBtn = S("button[type=submit]");
    
    public Send(string name) {
        nameInput.SetValue(name);
        submitBtn.Click();
    }
}
var form = new MyForm();
form.Send("John"); //now all browser actions will be called
```

## Configuration
Here is example `AppConfig.json` that need to be in project root path:
```json
{
  "DriverConfig": {
    "BrowserPath": "c:\\some\\chromium\\path\\chrome.exe",
    "DriverPath": "c:\\some\\chromedriver\\path\\chromedriver.exe",
    "Reconnect": false,
    "AutoClose": true,
    "Headless": false
  },
  "LocatorConfig": {
    "TimeoutMs": 20000,
    "ShortWaitMs": 3000,
    "HighlightActions": false
  }
}
```
Where: 
- **Reconnect** - should try to reuse same browser between runs.
- **AutoClose** - should browser be closed after test run - if you use reconnect, set it to false
- **Headless** - makes tests faster, but don't show browser UI
