public class ImageMetadata
{
    public ImageMetadata(
        string registry,
        string repository,
        string tag = "0.1.0")
    {
        Registry = registry;
        Repository = repository;
        Tag = tag;
    }

    public string Registry { get; set; }

    public string Repository { get; set; }

    public string Tag { get; set; }

    public string Name
        => $"{Registry}/{Repository}:{Tag}".ToLower();

    public override string ToString()
        => $"Repository: {Repository}\nTag: {Tag}\nName: {Name}";
}
