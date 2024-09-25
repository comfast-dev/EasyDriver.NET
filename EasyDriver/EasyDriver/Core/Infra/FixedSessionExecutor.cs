using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Comfast.EasyDriver.Core.Infra;

/// <summary> Replace NewSession command with fixed SessionId/Url to trick RemoteWebDriver constructor to reconnect to same browser.</summary>
internal class FixedSessionExecutor : HttpCommandExecutor {
    private readonly string _sessionId;

    public FixedSessionExecutor(string uri, string sessionId) : base(new Uri(uri), TimeSpan.FromSeconds(60)) {
        _sessionId = sessionId;
    }

    public override async Task<Response> ExecuteAsync(Command command) {
        return command.Name == DriverCommand.NewSession
            ? MockNewSession()
            : await base.ExecuteAsync(command);
    }

    public override Response Execute(Command command) {
        return Task.Run((Func<Task<Response>>)(() => ExecuteAsync(command))).GetAwaiter().GetResult();
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