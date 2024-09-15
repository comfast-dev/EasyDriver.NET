using Xunit.Abstractions;

namespace EasyDriver.Tests.Util;

public class UnitBase {
    protected readonly ITestOutputHelper _output;

    public UnitBase(ITestOutputHelper output) {
        _output = output;
    }
}