namespace EasyDriver.Tests.Unit;

public class EasyDriverConfigTest {
    [Fact] void ReadConfigTest() {
        Configuration.ReloadConfig("EasyDriverConfig.json");
    }
}