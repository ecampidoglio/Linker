public class PackageMetadata
{
    public PackageMetadata(
        string outputDirectory,
        string name,
        string version = "0.1.0",
        string extension = "zip")
    {
        OutputDirectory = outputDirectory;
        Name = name;
        Version = version;
        Extension = extension;
    }

    public DirectoryPath OutputDirectory { get; }

    public string Name { get; }

    public string Version { get; set; }

    public string Extension { get; set; }

    public string FileName
        => $"{Name}.{Version}.{Extension}";

    public string FullPath
        => OutputDirectory.CombineWithFilePath(FileName).FullPath;

    public override string ToString()
        => $"Name: {Name}\nVersion: {Version}\nPath: {FullPath}";
}
