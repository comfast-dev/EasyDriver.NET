namespace Comfast.EasyDriver.Core.Infra.Browser;

/// <summary> Custom client for WebDriver protocol. pre-alpha version. </summary>
public class WdClient {
    private HttpClient _client = new();
    private readonly string _baseUrl;
    public WdClient(string sessionString) {
        var(url, ssid) = WebDriverReconnectUtils.ParseSessionString(sessionString);
        _baseUrl = $"{url}session/{ssid}";
    }

    public HttpResponseMessage Title => Get("/title");

    private HttpResponseMessage Get(string endpoint) {
        return _client.GetAsync(_baseUrl + endpoint).Result;
    }
}