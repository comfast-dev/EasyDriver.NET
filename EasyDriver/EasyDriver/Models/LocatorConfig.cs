namespace Comfast.EasyDriver.Models;

/// <summary>
/// Configuration for locating elements logic
/// </summary>
public class LocatorConfig {
    /// <summary>
    /// If true - elements will be highlighted after actions like Click, SetValue, etc.
    /// </summary>
    public bool HighlightActions { get; set; } = false;

    /// <summary>
    /// Default timeout used in wait functions
    /// </summary>
    public int TimeoutMs { get; set; } = 20000;

    /// <summary>
    /// Default timeout for locating elements.
    /// </summary>
    public int? ShortWaitMs { get; set; } = 4000;

    /// <summary>
    /// Experimental flags
    /// </summary>
    public ExperimentalFeatures Experimental { get; set; } = new();
}

/// <summary>
/// Experimental configuration flags. Use with caution.
/// Not guaranteed that it works at every situation.
/// </summary>
public class ExperimentalFeatures {
    /// <summary>
    /// Use one call to JavaScript to get all texts instead of multiple Selenium calls.
    /// </summary>
    public bool GetTextsUsingJs { get; set; } = false;
}