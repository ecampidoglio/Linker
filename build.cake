#addin nuget:?package=Cake.Curl&version=2.1.0
#addin nuget:?package=Cake.Incubator&version=1.5.0

#load build/paths.cake
#load build/version.cake
#load build/urls.cake

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var packageOutputPath = Directory(Argument("packageOutputPath", "packages"));

var packageVersion = "0.1.0";

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore(Paths.SolutionFile.FullPath);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(
        Paths.ProjectFile.FullPath,
        new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
});

Task("Test")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreTest(Paths.TestProjectFile.FullPath);
});

Task("Version")
    .Does(() =>
{
    packageVersion = ReadVersionNumberFromProject(Context);
    Information($"Read package version {packageVersion}");
});

Task("Remove-Packages")
    .Does(() =>
{
    CleanDirectory(packageOutputPath);
});

Task("Package-NuGet")
    .IsDependentOn("Test")
    .IsDependentOn("Remove-Packages")
    .Does(() =>
{
    DotNetCorePack(
        Paths.ProjectFile.GetDirectory().FullPath,
        new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = packageOutputPath
        });
});

Task("Package-Zip")
    .IsDependentOn("Test")
    .IsDependentOn("Version")
    .IsDependentOn("Remove-Packages")
    .Does(() =>
{
    DotNetCorePublish(
        Paths.ProjectFile.GetDirectory().FullPath,
        new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = Paths.PublishDirectory
        });

    Zip(
        Paths.PublishDirectory,
        Combine(packageOutputPath, $"Linker.{packageVersion}.zip"));
});

Task("Deploy")
    .IsDependentOn("Package-Zip")
    .Does(() =>
{
    CurlUploadFile(
        Combine(packageOutputPath, $"Linker.{packageVersion}.zip"),
        EnvironmentVariable<Uri>("DeployTo", Urls.DeploymentUrl),
        new CurlSettings
        {
            Username = EnvironmentVariable("DeploymentUser"),
            Password = EnvironmentVariable("DeploymentPassword"),
            RequestCommand = "POST",
            ArgumentCustomization = args => args.Append("--fail")
        });
});

RunTarget(target);
