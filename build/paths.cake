public static class Paths
{
    public static FilePath SolutionFile => "src/Linker.sln";
    public static FilePath CodeCoverageResultFile => "coverage.xml";
    public static DirectoryPath CodeCoverageReportDirectory => "coverage";
    public static FilePath WebNuspecFile => "src/Web/Web.nuspec";
    public static FilePath WebProjectFile => "src/Web/Web.csproj";
}

public static FilePath Combine(DirectoryPath directory, FilePath file)
{
    return directory.CombineWithFilePath(file);
}

public DirectoryPath VS2017InstallDirectory(ICakeContext context)
{
    var programFilesX86 = context.Environment.GetSpecialPath(SpecialPath.ProgramFilesX86);
    string[] editions  = { "Enterprise", "Professional", "Community" };

    return editions
        .Select(edition => Directory($"{programFilesX86}/Microsoft Visual Studio/2017/{edition}"))
        .FirstOrDefault(path => context.DirectoryExists(path));
}
