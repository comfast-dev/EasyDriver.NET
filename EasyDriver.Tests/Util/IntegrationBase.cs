using Xunit.Abstractions;
using Xunit.Extensions.AssemblyFixture;

[assembly: TestFramework(AssemblyFixtureFramework.TypeName, AssemblyFixtureFramework.AssemblyName)]

namespace EasyDriver.Tests.Util;

public class IntegrationBase : IAssemblyFixture<IntegrationFixture> {
    protected readonly ITestOutputHelper _output;
    private readonly IntegrationFixture _fix;

    public IntegrationBase(ITestOutputHelper output, IntegrationFixture fix) {
        _output = output;
        _fix = fix;
    }
}

public class IntegrationFixture : IDisposable {
    //Before all hook

    //After all hook
    public void Dispose() {
        if (!Configuration.DriverConfig.Reconnect)
            Configuration.DriverProvider.CloseAllDrivers();
    }
}