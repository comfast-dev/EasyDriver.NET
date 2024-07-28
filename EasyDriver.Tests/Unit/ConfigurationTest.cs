namespace EasyDriver.Tests.Unit;

public class ConfigurationTest {
    [Fact] void ReadConfigTest() {
        Configuration.ReloadConfig("AppConfig.json");
    }
}