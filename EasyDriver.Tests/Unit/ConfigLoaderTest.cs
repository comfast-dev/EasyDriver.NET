using Comfast.Commons.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace EasyDriver.Tests.Unit;

[TestSubject(typeof(ConfigLoader))]
public class ConfigLoaderTest {
    private const string TestJson = "TestJson.json";

    [Fact] void ReadJson() {
        ConfigLoader.Load<string>(TestJson, "SomeString").Should().Be("onetwothree");
        ConfigLoader.Load<long>(TestJson, "SomeInt").Should().Be(123);
        ConfigLoader.Load<double>(TestJson, "SomeFloat").Should().Be(123.5);
        ConfigLoader.Load<bool>(TestJson, "SomeBool").Should().Be(true);
    }

    [Fact] void FailToReadJson() {
        ShouldThrow(() => ConfigLoader.Load<bool>(TestJson, "SomeInt"),
            "Unable to cast object of type 'System.Int64' to type 'System.Boolean'");

        ShouldThrow(() => ConfigLoader.Load<string>(TestJson, "notExist"),
            "Not found key 'notExist' in JSON: TestJson.json");

        ShouldThrow(() => ConfigLoader.Load<string>("notExist.json", "SomeString"),
            "Could not find file");
    }
}