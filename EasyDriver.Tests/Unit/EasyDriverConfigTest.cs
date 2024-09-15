namespace EasyDriver.Tests.Unit;

public class EasyDriverConfigTest {
    [Fact] void ReadConfigTest() {
        EasyDriverConfig.ReloadConfig("EasyDriverConfig.json");
    }
}