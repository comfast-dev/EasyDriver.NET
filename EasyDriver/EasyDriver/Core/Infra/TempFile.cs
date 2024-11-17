namespace Comfast.EasyDriver.Core.Infra;

public class TempFile {
    private readonly string _tempFilePath;

    public TempFile(string nameOrPath) {
        _tempFilePath = Path.Combine(Path.GetTempPath(), nameOrPath);
        var dir = Path.GetDirectoryName(_tempFilePath) ?? throw new("Invalid file directory: " + _tempFilePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        if (!File.Exists(_tempFilePath)) File.Create(_tempFilePath);
    }

    public void SaveFile(string content) => File.WriteAllText(_tempFilePath, content);
    public string ReadFile() => File.ReadAllText(_tempFilePath);
    public bool Exists => File.Exists(_tempFilePath);
}