using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Comfast.EasyDriver.Se.Infra;

internal class FixedSessionExecutor : HttpCommandExecutor {
    private readonly string _sessionId;

    public FixedSessionExecutor(string uri, string sessionId)
        : base(new Uri(uri), TimeSpan.FromSeconds(60)) {
        _sessionId = sessionId;
    }

    public override Response Execute(Command command) {
        return command.Name == DriverCommand.NewSession
            ? MockNewSession()
            : base.Execute(command);
    }

    private Response MockNewSession() {
        var response = new Response {
            SessionId = _sessionId,
            Status = WebDriverResult.Success,
            Value = new Dictionary<string, object>()
        };
        return response;
    }
}