public class PackageMetadata
{
    public PackageMetadata(
        string outputDirectory,
        string name,
        string version = "0.1.0")
    {
        OutputDirectory = outputDirectory;
        Name = name;
        Version = version;
    }

    public DirectoryPath OutputDirectory { get; }

    public string Name { get; }

    public string Version { get; set; }

    public string FileName
        => $"{Name}.{Version}.zip";

    public string FullPath
        => OutputDirectory.CombineWithFilePath(FileName).FullPath;
}