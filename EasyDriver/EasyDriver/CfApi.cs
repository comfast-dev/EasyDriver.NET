﻿using Comfast.Commons.Utils;
using Comfast.EasyDriver.Se;
using OpenQA.Selenium;

namespace Comfast.EasyDriver;
public class CfApi {
    public static IWebDriver Driver => Configuration.GetDriver();
    public static SeleniumLocator S(string cssOrXpath) => new(cssOrXpath);

    public static string CurrentUrl => Driver.Url;
    public static void NavigateTo(string url) => Driver.Navigate().GoToUrl(url);

    public static T ExecuteJs<T>(string jsCode, params object[] args) {
        var jsDriver = (IJavaScriptExecutor)Driver;
        return (T)jsDriver.ExecuteScript(jsCode, args);
    }

    public static void WaitFor(Func<bool> action, string? description = null, int? timeoutMs = null) {
        WaitUtils.WaitFor(action, description, timeoutMs);
    }
}


