#addin nuget:?package=Cake.Curl&version=4.0.0
#addin nuget:?package=Cake.Incubator&version=5.0.1

#load build/paths.cake
#load build/version.cake
#load build/urls.cake

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var packageOutputPath = Directory(Argument("packageOutputPath", "packages"));

var packageVersion = "0.1.0";

Task("Clean")
    .Does(() =>
{
    CleanDirectory(packageOutputPath);
    CleanDirectory(Paths.TestResultsDirectory);
    CleanDirectory(Paths.PublishDirectory);
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
});

Task("Build")
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
    .Does(() =>
{
    CleanDirectory(Paths.TestResultsDirectory);

    DotNetCoreTest(
        Paths.TestProjectFile.FullPath,
        new DotNetCoreTestSettings
        {
            Configuration = configuration,
            Logger = "trx", // VSTest results format
            ResultsDirectory = Paths.TestResultsDirectory
        });
});

Task("Version")
    .Does(() =>
{
    packageVersion = ReadVersionNumberFromProject(Context);
    Information($"Read package version {packageVersion}");
});

Task("Package-NuGet")
    .IsDependentOn("Test")
    .Does(() =>
{
    CleanDirectory(packageOutputPath);

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
    .Does(() =>
{
    CleanDirectory(packageOutputPath);

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

Task("Publish-AzurePipelines-Test-Results")
    .IsDependentOn("Test")
    .WithCriteria(() => BuildSystem.IsRunningOnAzurePipelinesHosted)
    .Does(() =>
{
    BuildSystem.TFBuild.Commands.PublishTestResults(
        new TFBuildPublishTestResultsData
        {
            Configuration = configuration,
            TestRunner = TFTestRunnerType.VSTest,
            MergeTestResults = true,
            TestResultsFiles = GetFiles(Paths.TestResultsDirectory + "/*.trx").ToList()
        });
});

Task("Publish-Test-Results")
    .IsDependentOn("Publish-AzurePipelines-Test-Results");

RunTarget(target);
