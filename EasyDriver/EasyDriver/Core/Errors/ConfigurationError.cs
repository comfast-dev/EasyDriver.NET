namespace Comfast.EasyDriver.Core.Errors;

public class ConfigurationError(string msg) : Exception(msg) { }