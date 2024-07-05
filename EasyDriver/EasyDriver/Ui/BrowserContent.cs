namespace Comfast.EasyDriver.Ui;

public class BrowserContent {

    /// <summary>
    /// Copy given resource file to temp folder and open it in browser.
    /// </summary>
    /// <param name="resourcePath">path to resource file, where root is "src/test/resources"</param>
    public void OpenResourceFile(String resourcePath) {
        var path = Path.Combine(Directory.GetCurrentDirectory(), resourcePath);
        DriverApi.NavigateTo(path);
    }

    /// <summary>
    /// Set body tag content
    /// </summary>
    public void SetBody(string bodyHtml) {
        DriverApi.ExecuteJs<object>("document.querySelector('html>body').innerHTML = arguments[0]", bodyHtml);
    }
}