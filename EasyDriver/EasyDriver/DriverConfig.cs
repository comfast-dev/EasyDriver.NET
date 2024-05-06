namespace Comfast.EasyDriver;

public class DriverConfig {
    public string BrowserPath { get; set; }
    public string DriverPath { get; set; }
    public bool Reconnect { get; set; }
    public bool AutoClose { get; set; }
    public bool Headless { get; set; } = false;
    
    public bool HighlightActions { get; set; }
    public int TimeoutMs { get; set; }
    public int? ShortWaitMs { get; set; }
}