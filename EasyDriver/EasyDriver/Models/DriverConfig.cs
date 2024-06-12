﻿namespace Comfast.EasyDriver.Models;

public class DriverConfig {
    public string BrowserPath { get; set; }
    public string DriverPath { get; set; }
    public bool Reconnect { get; set; }
    public bool AutoClose { get; set; }
    public bool Headless { get; set; } = false;
    public string BrowserName { get; set; } = "chrome";
}