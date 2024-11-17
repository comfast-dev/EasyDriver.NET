using Comfast.Commons.Utils;
using Comfast.EasyDriver.Core.Errors;
using Microsoft.Extensions.Configuration;

namespace Comfast.EasyDriver.Models;

/// <summary>
/// Main source of truth for framework.
/// Internal fields of BrowserConfig/RuntimeConfig can be edited in runtime.
/// </summary>
public class EasyDriverConfig {
    const string ConfigFileName = "EasyDriverConfig.json";

    public EasyDriverConfig() {
        if (!File.Exists(ConfigFileName)) throw new ConfigurationError(
            $"Not found config file: {ConfigFileName}");
        ReloadConfig(ConfigFileName);
    }

    /// <summary> Options that define way how WebDriver managed browser is created/managed</summary>
    public BrowserConfig BrowserConfig { get; } = new();

    /// <summary> Feature flags / timeouts</summary>
    public RuntimeConfig RuntimeConfig { get; } = new();

    /// <summary> Reloads Configuration from given Config file.</summary>
    /// <param name="filePath"> e.g. MyAppConfig.json</param>
    public void ReloadConfig(string filePath) {
        ReloadConfig(new ConfigurationBuilder().AddJsonFile(filePath).Build());
    }

    /// <summary> Reloads configuration.</summary>
    /// <param name="conf">IConfiguration object</param>
    public void ReloadConfig(IConfiguration conf) {
        BrowserConfig.RewriteAllFieldsFrom(conf.GetSection("BrowserConfig").Get<BrowserConfig>());
        RuntimeConfig.RewriteAllFieldsFrom(conf.GetSection("RuntimeConfig").Get<RuntimeConfig>());
    }
}