namespace Comfast.EasyDriver.Js;

public static class JsFiles {
    public static string ReadJsFile(string jsFileName) {
        return File.ReadAllText("EasyDriver\\Js\\" + jsFileName);
    }
}