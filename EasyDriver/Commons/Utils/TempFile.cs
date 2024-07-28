namespace Comfast.Commons.Utils;

public class TempFile {
    public string FilePath { get; }

    public TempFile(string name) {
        FilePath = Path.Combine(Path.GetTempPath(), name);
    }

    public void Write(string content) {
        // Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        // if (!File.Exists(FilePath)) File.Create(FilePath);
        using var sw = new StreamWriter(FilePath);
        sw.Write(content);
    }

    public string Read() {
        using var sr = new StreamReader(FilePath);
        return sr.ReadToEnd();
    }
}