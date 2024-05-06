using Comfast.Commons.Utils;

namespace Comfast.EasyDriver.Ui;

public class BrowserContent {
    /**
     * Copy given resource file to temp folder and open it in browser.
     * @param resourcePath path to resource file, where root is "src/test/resources"
     */
    public void OpenResourceFile(String resourcePath) {
        var path = Path.Combine(Directory.GetCurrentDirectory(), resourcePath);
        CfApi.NavigateTo(path);
    }

    // /**
    //  * Set style tag to head section
    //  */
    // public BrowserContent setStyle(@Language("CSS") String styleContent) {
    //     executeJs("document.querySelector('html>head').innerHTML = arguments[0]",
    //         "<style>" + styleContent + "</style>");
    //     return this;
    // }
    //
    
    /// <summary>
    /// Set body tag content
    /// </summary>
    public BrowserContent SetBody(string bodyHtml) {
        CfApi.ExecuteJs<object>("document.querySelector('html>body').innerHTML = arguments[0]", bodyHtml);
        return this;
    }
    // /**
    //  * Add $ and $$ JS methods to current page for convenience
    //  */
    // public BrowserContent addJsTools() {
    //     setScriptTag(
    //         "const $$ = (css, parent = document) => Array.from(parent.querySelectorAll(css));\n" +
    //         "const $ =  (css, parent = document) => parent.querySelector(css);\n"
    //         );
    //     return this;
    // }
    //
    // /**
    //  * Adds script tag to head section
    //  */
    // public BrowserContent setScriptTag(@Language("JavaScript") String scriptContent) {
    //     String escapedScriptContent = scriptContent.replace("`", "\\`");
    //     @Language("JavaScript") final String ADD_SCRIPT =
    //         "const s = document.createElement('script');" +
    //         "s.innerHTML = `" + escapedScriptContent + "`;" +
    //         " document.querySelector('head').append(s);";
    //     executeJs(ADD_SCRIPT);
    //     return this;
    // }
    //
    // /**
    //  * Clear all html content
    //  */
    // public void clearAll() {
    //     executeJs("document.querySelector('html').innerHTML = ''");
    // }
    //
    // /**
    //  * Handle Exception:
    //  * Opens a new page if the current one is secured by TrustedHTML
    //  */
    // private Object executeJs(String script, params object[] args) {
    //     try {
    //         return CfApi.ExecuteJs<object>(script, args);
    //     } catch(JavaScriptException e) {
    //         if(e.Message.Contains("This document requires 'TrustedHTML' assignment")) {
    //             CfApi.NavigateTo("about:blank");
    //             return CfApi.ExecuteJs(script, args);
    //         }
    //         throw e;
    //     }
    // }
}