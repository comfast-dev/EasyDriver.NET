namespace EasyDriver.Tests.Util.Hooks;

public class AssemblyFixture : IDisposable {
    //Before all hook
    public AssemblyFixture() { }

    //After all hook
    public void Dispose() {
        if (!Configuration.BrowserConfig.Reconnect) {
            DriverProvider.CloseAllDrivers();
        }
    }
}