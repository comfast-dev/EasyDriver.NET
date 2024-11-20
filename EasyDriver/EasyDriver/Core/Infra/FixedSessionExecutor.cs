using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Comfast.EasyDriver.Core.Infra;

/// <summary>
/// 1. Replace NewSession command with fixed SessionId/Url
///    to trick RemoteWebDriver constructor to reconnect to same browser.
/// 2. Add Events feature.
/// </summary>
internal class FixedSessionExecutor(string uri, string sessionId)
    : HttpCommandExecutor(new(uri), TimeSpan.FromSeconds(60)) {

    public override Response Execute(Command command) {
        return WebDriverEvents.CallAction(this, command.Name, () => HandleCommand(command));
    }

    private Response HandleCommand(Command command) {
        return command.Name == DriverCommand.NewSession
            ? MockNewSession()
            : base.Execute(command);
    }

    public async override Task<Response> ExecuteAsync(Command command) {
        return await WebDriverEvents.CallActionAsync(this, command.Name, () => HandleCommandAsync(command), command.Parameters);
    }

    private async Task<Response> HandleCommandAsync(Command command) {
        return command.Name == DriverCommand.NewSession
            ? MockNewSession()
            : await base.ExecuteAsync(command);
    }

    private Response MockNewSession() {
        return new() {
            SessionId = sessionId,
            Status = WebDriverResult.Success,
            Value = new Dictionary<string, object>()
        };
    }
}