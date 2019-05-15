#tool nuget:?package=coveralls.io&version=1.4.2

#addin nuget:?package=Cake.Curl&version=4.0.0
#addin nuget:?package=Cake.Incubator&version=5.0.1
#addin nuget:?package=Cake.Coverlet&version=2.2.1
#addin nuget:?package=Cake.Coveralls&version=0.9.0

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

Task("Compile")
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

    var coverageSettings = new CoverletSettings
    {
        CollectCoverage = true,
        CoverletOutputDirectory = Paths.CodeCoverageReportFile.GetDirectory(),
        CoverletOutputName = Paths.CodeCoverageReportFile.GetFilename().ToString()
    }
    .WithFilter("[Linker.*Tests]*");

    if (BuildSystem.IsRunningOnTeamCity)
    {
        coverageSettings.CoverletOutputFormat = CoverletOutputFormat.teamcity;
    }
    else if (BuildSystem.IsRunningOnAzurePipelinesHosted)
    {
        coverageSettings.CoverletOutputFormat = CoverletOutputFormat.cobertura;
    }
    else
    {
        coverageSettings.CoverletOutputFormat = CoverletOutputFormat.opencover;
    }

    DotNetCoreTest(
        Paths.TestProjectFile.FullPath,
        new DotNetCoreTestSettings
        {
            Configuration = configuration,
            Logger = "trx", // VSTest results format
            ResultsDirectory = Paths.TestResultsDirectory
        },
        coverageSettings);
});

Task("Version")
    .Does(() =>
{
    packageVersion = ReadVersionFromProjectFile(Context);
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
    .WithCriteria(context => context.LatestCommitHasVersionTag(), "The latest commit doesn't have a version tag")
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

Task("Publish-AzurePipelines-Code-Coverage-Report")
    .IsDependentOn("Test")
    .WithCriteria(() => FileExists(Paths.CodeCoverageReportFile))
    .WithCriteria(() => BuildSystem.IsRunningOnAzurePipelinesHosted)
    .Does(() =>
{
    TFBuild.Commands.PublishCodeCoverage(
        new TFBuildPublishCodeCoverageData
        {
            CodeCoverageTool = TFCodeCoverageToolType.Cobertura,
            SummaryFileLocation = Paths.CodeCoverageReportFile
        }
    );
});

Task("Publish-TeamCity-Test-Results")
    .IsDependentOn("Test")
    .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
    .Does(() =>
{
    var testResults = GetFiles(Paths.TestResultsDirectory + "/*.trx");

    foreach (var result in testResults)
    {
        TeamCity.ImportData("vstest", result);
    }
});

Task("Publish-Coveralls-Code-Coverage-Report")
    .IsDependentOn("Test")
    .WithCriteria(() => FileExists(Paths.CodeCoverageReportFile))
    .WithCriteria(() => BuildSystem.IsRunningOnAppVeyor)
    .Does(() =>
{
    CoverallsIo(
        Paths.CodeCoverageReportFile,
        new CoverallsIoSettings
        {
            RepoToken = EnvironmentVariable("CoverallsRepoToken")
        });
});

Task("Publish-AzurePipelines-Artifacts")
    .IsDependentOn("Package-Zip")
    .WithCriteria(() => BuildSystem.IsRunningOnAzurePipelinesHosted)
    .Does(() =>
{
    TFBuild.Commands.UploadArtifactDirectory(packageOutputPath);
});

Task("Publish-AppVeyor-Artifacts")
    .IsDependentOn("Package-Zip")
    .WithCriteria(() => BuildSystem.IsRunningOnAppVeyor)
    .Does(() =>
{
    foreach (var package in GetFiles(packageOutputPath))
    {
        AppVeyor.UploadArtifact(package);
    }
});

Task("Publish-TeamCity-Artifacts")
    .IsDependentOn("Package-Zip")
    .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
    .Does(() =>
{
    TeamCity.PublishArtifacts(packageOutputPath);
});

Task("Set-Build-Number")
    .IsDependentOn("Version")
    .WithCriteria(() => !BuildSystem.IsLocalBuild)
    .Does(() =>
{
    if (BuildSystem.IsRunningOnAzurePipelinesHosted)
    {
        TFBuild.Commands.UpdateBuildNumber(
            $"{packageVersion}+{TFBuild.Environment.Build.Number}");
    }
    else if (BuildSystem.IsRunningOnAppVeyor)
    {
        AppVeyor.UpdateBuildVersion(
            $"{packageVersion}+{AppVeyor.Environment.Build.Number}");
    }
    else if (BuildSystem.IsRunningOnTeamCity)
    {
        TeamCity.SetBuildNumber(
            $"{packageVersion}+{TeamCity.Environment.Build.Number}");
    }
});

Task("Publish-Test-Results")
    .IsDependentOn("Publish-AzurePipelines-Test-Results")
    .IsDependentOn("Publish-AzurePipelines-Code-Coverage-Report")
    .IsDependentOn("Publish-TeamCity-Test-Results")
    .IsDependentOn("Publish-Coveralls-Code-Coverage-Report");

Task("Publish-Artifacts")
    .IsDependentOn("Publish-AzurePipelines-Artifacts")
    .IsDependentOn("Publish-AppVeyor-Artifacts")
    .IsDependentOn("Publish-TeamCity-Artifacts");

Task("Build")
    .IsDependentOn("Compile")
    .IsDependentOn("Test");

Task("Build-CI")
    .IsDependentOn("Compile")
    .IsDependentOn("Test")
    .IsDependentOn("Version")
    .IsDependentOn("Package-Zip")
    .IsDependentOn("Publish-Test-Results")
    .IsDependentOn("Publish-Artifacts")
    .IsDependentOn("Set-Build-Number");

Task("Deploy-CI")
    .IsDependentOn("Deploy")
    .IsDependentOn("Publish-Artifacts")
    .IsDependentOn("Set-Build-Number");

RunTarget(target);
