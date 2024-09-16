namespace Comfast.EasyDriver.Models;

/// <summary> Configuration for locating elements logic</summary>
public class RuntimeConfig {
    /// <summary> Default timeout used in wait functions</summary>
    public int TimeoutMs { get; set; } = 20000;

    /// <summary> Time between retries during Wait methods </summary>
    public int PoolingTimeMs { get; set; } = 100;

    /// <summary> Default timeout for locating elements.</summary>
    public int? ShortWaitMs { get; set; } = 4000;

    /// <summary> If true - elements will be highlighted after actions like Click, SetValue, etc.</summary>
    public bool HighlightActions { get; set; } = false;

    /// <summary> Use one call to JavaScript to get all texts instead of multiple Selenium calls.</summary>
    public bool ExperimentalGetTextUsingJs { get; set; } = false;
}