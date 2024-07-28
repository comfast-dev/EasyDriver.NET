namespace Comfast.Commons.Utils;

public class TempFile {
    public string FilePath { get; }

    /// <summary> Initialize, prepare file directory.</summary>
    public TempFile(string nameOrPath) {
        FilePath = Path.Combine(Path.GetTempPath(), nameOrPath);

        var dir = Path.GetDirectoryName(FilePath) ?? throw new(
            "Invalid file directory: " + FilePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        if (!File.Exists(FilePath)) File.Create(FilePath);
    }

    /// <summary>Write content to file. Replace all. </summary>
    public void Write(string content) => File.WriteAllText(FilePath, content);

    /// <summary> Read file text.</summary>
    public string Read() => File.ReadAllText(FilePath);

    /// <summary> Delete temp file </summary>
    public void Delete() => File.Delete(FilePath);
}