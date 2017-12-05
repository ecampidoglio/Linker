#tool nuget:?package=xunit.runner.console&version=2.2.0
#tool nuget:?package=xunit.runner.visualstudio&version=2.2.0
#tool nuget:?package=OpenCover&version=4.6.519
#tool nuget:?package=JetBrains.dotCover.CommandLineTools&version=2016.2.20160913.100041
#tool nuget:?package=ReportGenerator&version=2.5.8
#tool nuget:?package=GitVersion.CommandLine&version=3.6.5
#tool nuget:?package=OctopusTools&version=4.21.0

#addin nuget:?package=Cake.WebDeploy&version=0.2.4

#load build/paths.cake
#load build/urls.cake

var target = Argument("Target", "Build");
var configuration = Argument("Configuration", "Release");
var codeCoverageReportPath = Argument<FilePath>("CodeCoverageReportPath", "coverage.zip");
var packageOutputPath = Argument<DirectoryPath>("PackageOutputPath", "packages");
var deploymentUser = Argument<string>("DeploymentUser", null);
var deploymentPassword = Argument<string>("DeploymentPassword", null);

var packageVersion = "0.1.0";
var packagePath = File("Linker.zip").Path;

Task("Restore")
    .Does(() =>
{
    NuGetRestore(Paths.SolutionFile);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetBuild(
        Paths.SolutionFile,
        settings => settings.SetConfiguration(configuration)
                            .WithTarget("Build"));
});

Task("Test-OpenCover")
    .IsDependentOn("Build")
    .WithCriteria(() => BuildSystem.IsLocalBuild || BuildSystem.IsRunningOnAppVeyor)
    .Does(() =>
{
    OpenCover(
        tool => tool.XUnit2(
            $"**/bin/{configuration}/*Tests.dll",
            new XUnit2Settings
            {
                ShadowCopy = false
            }),
        Paths.CodeCoverageResultFile,
        new OpenCoverSettings()
            .WithFilter("+[Linker.*]*")
            .WithFilter("-[Linker.*Tests*]*")
    );
});

Task("Report-Coverage")
    .IsDependentOn("Test-OpenCover")
    .WithCriteria(() => BuildSystem.IsLocalBuild)
    .Does(() =>
{
    ReportGenerator(
        Paths.CodeCoverageResultFile,
        Paths.CodeCoverageReportDirectory,
        new ReportGeneratorSettings
        {
            ReportTypes = new[] { ReportGeneratorReportType.Html }
        }
    );

    Zip(
        Paths.CodeCoverageReportDirectory,
        MakeAbsolute(codeCoverageReportPath)
    );
});

Task("Version")
    .Does(() =>
{
    var version = GitVersion();
    Information($"Calculated semantic version {version.SemVer}");

    packageVersion = version.NuGetVersion;
    Information($"Corresponding package version {packageVersion}");

    if (!BuildSystem.IsLocalBuild)
    {
        GitVersion(new GitVersionSettings
        {
            OutputType = GitVersionOutput.BuildServer,
            UpdateAssemblyInfo = true
        });
    }
});

Task("Remove-Packages")
    .Does(() =>
{
    CleanDirectory(packageOutputPath);
});

Task("Package-NuGet")
    .IsDependentOn("Test")
    .IsDependentOn("Version")
    .IsDependentOn("Remove-Packages")
    .Does(() =>
{
    EnsureDirectoryExists(packageOutputPath);

    NuGetPack(
        Paths.WebNuspecFile,
        new NuGetPackSettings
        {
            Version = packageVersion,
            OutputDirectory = packageOutputPath,
            NoPackageAnalysis = true
        });
});

Task("Package-WebDeploy")
    .IsDependentOn("Test")
    .IsDependentOn("Version")
    .IsDependentOn("Remove-Packages")
    .Does(() =>
{
    EnsureDirectoryExists(packageOutputPath);
    packagePath = Combine(MakeAbsolute(packageOutputPath), $"Linker.{packageVersion}.zip");

    MSBuild(
        Paths.WebProjectFile,
        settings =>
            settings.SetConfiguration(configuration)
                    .WithTarget("Package")
                    .WithProperty("PackageLocation", packagePath.FullPath));
});

Task("Deploy-OctopusDeploy")
    .IsDependentOn("Package-NuGet")
    .Does(() =>
{
    OctoPush(
        Urls.OctopusServerUrl,
        EnvironmentVariable("OctopusApiKey"),
        GetFiles($"{packageOutputPath}/*.nupkg"),
        new OctopusPushSettings
        {
            ReplaceExisting = true
        });

    OctoCreateRelease(
        "Linker",
        new CreateReleaseSettings
        {
            Server = Urls.OctopusServerUrl,
            ApiKey = EnvironmentVariable("OctopusApiKey"),
            ReleaseNumber = packageVersion,
            DefaultPackageVersion = packageVersion,
            DeployTo = "Test",
            WaitForDeployment = true
        });
});

Task("Deploy-WebDeploy")
    .IsDependentOn("Package-WebDeploy")
    .IsDependentOn("Publish-Artifacts")
    .Does(() =>
{
    var userName = string.IsNullOrEmpty(deploymentUser)
        ? EnvironmentVariable("DeploymentUser")
        : deploymentUser;
    var password = string.IsNullOrEmpty(deploymentPassword)
        ? EnvironmentVariable("DeploymentPassword")
        : deploymentPassword;

    DeployWebsite(new DeploySettings()
        .SetPublishUrl(Urls.WebDeployPublishUrl)
        .FromSourcePath(packagePath.FullPath)
        .ToDestinationPath("site/wwwroot/Linker")
        .UseSiteName("Linker-Demo")
        .AddParameter("IIS Web Application Name", "Linker-Demo")
        .UseUsername(userName)
        .UsePassword(password));
});

Task("Test-DotCover")
    .IsDependentOn("Build")
    .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
    .Does(() =>
{
    DotCoverCover(
        tool => tool.XUnit2(
            $"**/bin/{configuration}/*Tests.dll",
            new XUnit2Settings
            {
                ShadowCopy = false
            }),
        Paths.CodeCoverageResultFile,
        new DotCoverCoverSettings()
            .WithFilter("+:Linker*")
            .WithFilter("-:Linker.Tests")
    );

    TeamCity.ImportDotCoverCoverage(MakeAbsolute(Paths.CodeCoverageResultFile));
});

Task("Test-VSTest")
    .IsDependentOn("Build")
    .WithCriteria(() => BuildSystem.IsRunningOnVSTS)
    .Does(() =>
{
    VSTest(
        GetFiles($"**/bin/{configuration}/*Tests.dll"),
        new VSTestSettings
        {
            Logger = "trx",
            EnableCodeCoverage = true,
            InIsolation = true,
            TestAdapterPath = $"tools/xunit.runner.visualstudio/build/_common",
            ToolPath = $"{VS2017InstallDirectory(Context)}/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe"
        });
});

Task("Publish-Artifacts-TeamCity")
    .IsDependentOn("Package-WebDeploy")
    .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
    .Does(() =>
{
    BuildSystem.TeamCity.PublishArtifacts(packagePath.FullPath);
});

Task("Publish-Artifacts-VSTS")
    .IsDependentOn("Package-WebDeploy")
    .WithCriteria(() => BuildSystem.IsRunningOnVSTS)
    .Does(() =>
{
    BuildSystem.TFBuild.Commands.UploadArtifact(
        "Packages",
        packagePath,
        $"Linker.{packageVersion}");
});

Task("Publish-Artifacts-AppVeyor")
    .IsDependentOn("Package-WebDeploy")
    .WithCriteria(() => BuildSystem.IsRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UploadArtifact(
        packagePath,
        settings => settings
            .SetDeploymentName(configuration)
            .SetArtifactType(AppVeyorUploadArtifactType.WebDeployPackage)
    );
});

Task("Test")
    .IsDependentOn("Test-OpenCover")
    .IsDependentOn("Test-DotCover")
    .IsDependentOn("Test-VSTest");

Task("Publish-Artifacts")
    .IsDependentOn("Publish-Artifacts-TeamCity")
    .IsDependentOn("Publish-Artifacts-VSTS")
    .IsDependentOn("Publish-Artifacts-AppVeyor");

RunTarget(target);
