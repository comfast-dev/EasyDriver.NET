using EasyDriver.Tests.Util;
using EasyDriver.Tests.Util.Hooks;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration.Ui;

public class LabeledComponentTests : IntegrationBase {
    public LabeledComponentTests(ITestOutputHelper output, AssemblyFixture fix) : base(output, fix) { }

    [Fact] void LabeledTest() { }
}