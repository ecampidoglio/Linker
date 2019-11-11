#tool nuget:?package=GitVersion.CommandLine&version=5.0.1
#tool nuget:?package=coveralls.io&version=1.4.2
#tool nuget:?package=OctopusTools&version=6.7.0

#addin nuget:?package=Cake.Npm&version=0.17.0
#addin nuget:?package=Cake.Curl&version=4.1.0
#addin nuget:?package=Cake.Incubator&version=5.0.1
#addin nuget:?package=Cake.Coverlet&version=2.2.1
#addin nuget:?package=Cake.Coveralls&version=0.9.0
#addin nuget:?package=Cake.Docker&version=0.10.1

#load build/container.cake
#load build/paths.cake
#load build/package.cake
#load build/version.cake
#load build/urls.cake

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var deployToEnvironment = Argument("deployToEnvironment", "Development");

Setup<PackageMetadata>(context =>
{
    var metadata = new PackageMetadata(
        outputDirectory: Argument("packageOutputPath", "packages"),
        name: "Linker",
        version: ReadVersionFromProjectFile(Context));

    Information($"Package\n{metadata}");

    return metadata;
});

Setup<ImageMetadata>(context =>
{
    var metadata = new ImageMetadata(
        registry: Urls.DockerRegistryUrl,
        repository: "Linker",
        tag: ReadVersionFromProjectFile(Context));

    Information($"Image\n{metadata}");

    return metadata;
});

Task("Clean")
    .Does<PackageMetadata>(package =>
{
    CleanDirectory(package.OutputDirectory);
    CleanDirectory(Paths.TestResultsDirectory);
    CleanDirectory(Paths.PublishDirectory);
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
    CleanDirectories("**/node_modules");
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
    .IsDependentOn("Compile")
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

    coverageSettings.CoverletOutputFormat = BuildSystem switch
    {
        { IsRunningOnTeamCity: true } => CoverletOutputFormat.teamcity,
        { IsRunningOnAzurePipelinesHosted: true } => CoverletOutputFormat.cobertura,
        _ => CoverletOutputFormat.opencover
    };

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
    .Does<PackageMetadata>(package =>
{
    package.Version ??= GitVersion().FullSemVer;

    Information($"Version set to {package.Version}");
});

Task("Build-Frontend")
    .Does(() =>
{
    NpmInstall(settings => settings.FromPath(Paths.ProjectDirectory));

    NpmRunScript("build", settings => settings.FromPath(Paths.ProjectDirectory));
});

Task("Package-NuGet")
    .IsDependentOn("Test")
    .IsDependentOn("Build-Frontend")
    .Does<PackageMetadata>(package =>
{
    package.Extension = "nupkg";

    DotNetCorePack(
        Paths.ProjectFile.GetDirectory().FullPath,
        new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = package.OutputDirectory,
            NoRestore = true,
            NoBuild = true
        });
});

Task("Package-WebDeploy")
    .WithCriteria(IsRunningOnWindows(), "MSDeploy is only available on Windows")
    .IsDependentOn("Test")
    .IsDependentOn("Build-Frontend")
    .IsDependentOn("Version")
    .Does<PackageMetadata>(package =>
{
    package.Extension = "zip";

    DotNetCoreMSBuild(
        Paths.ProjectFile.FullPath,
        new DotNetCoreMSBuildSettings()
            .SetConfiguration(configuration)
            .WithProperty("DeployOnBuild", "true")
            .WithProperty("WebPublishMethod", "Package")
            .WithProperty("PackageLocation", MakeAbsolute(File(package.FullPath)).FullPath)
    );
});

Task("Package-Zip")
    .IsDependentOn("Test")
    .IsDependentOn("Build-Frontend")
    .IsDependentOn("Version")
    .Does<PackageMetadata>(package =>
{
    package.Extension = "zip";

    DotNetCorePublish(
        Paths.ProjectFile.GetDirectory().FullPath,
        new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = Paths.PublishDirectory,
            NoRestore = true,
            NoBuild = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings
            {
                NoLogo = true
            }
        });

    Zip(Paths.PublishDirectory, package.FullPath);
});

Task("Package-Octopus")
    .IsDependentOn("Test")
    .IsDependentOn("Build-Frontend")
    .IsDependentOn("Version")
    .Does<PackageMetadata>(package =>
{
    package.Extension = "nupkg";

    DotNetCorePublish(
        Paths.ProjectFile.GetDirectory().FullPath,
        new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = Paths.PublishDirectory,
            NoRestore = true,
            NoBuild = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings
            {
                NoLogo = true
            }
        });

    OctoPack(
        package.Name,
        new OctopusPackSettings {
            Format = OctopusPackFormat.NuPkg,
            Version = package.Version,
            BasePath = Paths.PublishDirectory,
            OutFolder = package.OutputDirectory
        });
});

Task("Package-Docker")
    .IsDependentOn("Test")
    .IsDependentOn("Build-Frontend")
    .IsDependentOn("Version")
    .Does<ImageMetadata>(image =>
{
    DotNetCorePublish(
        Paths.ProjectFile.GetDirectory().FullPath,
        new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = Paths.PublishDirectory,
            NoRestore = true,
            NoBuild = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings
            {
                NoLogo = true
            }
        });

    DockerBuild(
        new DockerImageBuildSettings
        {
            Tag = new[] { image.Name }
        },
        Paths.DockerContextDirectory.FullPath);
});

Task("Deploy-WebDeploy")
    .WithCriteria(IsRunningOnWindows(), "MSDeploy is only available on Windows")
    .IsDependentOn("Package-WebDeploy")
    .Does(() =>
{
    DotNetCoreMSBuild(
        Paths.ProjectFile.FullPath,
        new DotNetCoreMSBuildSettings()
            .WithProperty("DeployOnBuild", "true")
            .WithProperty("WebPublishMethod", "MSDeploy")
            .WithProperty("MSDeployServiceURL", Urls.WebDeployUrl.ToString())
            .WithProperty("DeployIisAppPath", "Linker-Demo")
            .WithProperty("UserName", EnvironmentVariable("DeploymentUser"))
            .WithProperty("Password", EnvironmentVariable("DeploymentPassword"))
    );
});

Task("Deploy-Kudu")
    .WithCriteria(context => context.LatestCommitHasVersionTag(), "The latest commit doesn't have a version tag")
    .IsDependentOn("Package-Zip")
    .Does<PackageMetadata>(package =>
{
    CurlUploadFile(
        package.FullPath,
        EnvironmentVariable<Uri>("DeployTo", Urls.KuduDeployUrl),
        new CurlSettings
        {
            Username = EnvironmentVariable("DeploymentUser"),
            Password = EnvironmentVariable("DeploymentPassword"),
            RequestCommand = "POST",
            ArgumentCustomization = args => args.Append("--fail")
        });
});

Task("Deploy-Octopus")
    .IsDependentOn("Package-Octopus")
    .Does<PackageMetadata>(package =>
{
    OctoPush(
        Urls.OctopusDeployServerUrl.AbsoluteUri,
        EnvironmentVariable("OctopusApiKey"),
        package.FullPath,
        new OctopusPushSettings
        {
            EnableServiceMessages = true
        });

    OctoCreateRelease(
        "Linker",
        new CreateReleaseSettings
        {
            Server = Urls.OctopusDeployServerUrl.AbsoluteUri,
            ApiKey = EnvironmentVariable("OctopusApiKey"),
            ReleaseNumber = package.Version,
            DefaultPackageVersion = package.Version,
            DeployTo = deployToEnvironment,
            IgnoreExisting = true,
            DeploymentProgress = true,
            WaitForDeployment = true
        });
});

Task("Deploy-Docker")
    .IsDependentOn("Package-Docker")
    .Does<ImageMetadata>(image =>
{
    DockerLogin(
        EnvironmentVariable("DockerUser"),
        EnvironmentVariable("DockerPassword"),
        Urls.DockerRegistryUrl);

    DockerPush(image.Name);
})
.Finally(() =>
{
    DockerLogout(Urls.DockerRegistryUrl);
});

Task("Publish-AzurePipelines-Test-Results")
    .WithCriteria(() => BuildSystem.IsRunningOnAzurePipelinesHosted)
    .IsDependentOn("Test")
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
    .WithCriteria(() => FileExists(Paths.CodeCoverageReportFile))
    .WithCriteria(() => BuildSystem.IsRunningOnAzurePipelinesHosted)
    .IsDependentOn("Test")
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
    .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
    .IsDependentOn("Test")
    .Does(() =>
{
    var testResults = GetFiles(Paths.TestResultsDirectory + "/*.trx");

    foreach (var result in testResults)
    {
        TeamCity.ImportData("vstest", result);
    }
});

Task("Publish-Coveralls-Code-Coverage-Report")
    .WithCriteria(() => FileExists(Paths.CodeCoverageReportFile))
    .WithCriteria(() => BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Test")
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
    .WithCriteria(() => BuildSystem.IsRunningOnAzurePipelinesHosted)
    .IsDependentOn("Package-Zip")
    .Does<PackageMetadata>(package =>
{
    TFBuild.Commands.UploadArtifactDirectory(package.OutputDirectory);
});

Task("Publish-AppVeyor-Artifacts")
    .WithCriteria(() => BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Package-Zip")
    .Does<PackageMetadata>(package =>
{
    foreach (var packageFile in GetFiles(package.OutputDirectory + "/*.zip"))
    {
        AppVeyor.UploadArtifact(packageFile);
    }
});

Task("Publish-TeamCity-Artifacts")
    .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
    .IsDependentOn("Package-Zip")
    .Does<PackageMetadata>(package =>
{
    TeamCity.PublishArtifacts(package.OutputDirectory.FullPath);
});

Task("Publish-NuGet-Package-To-GitHub")
    .WithCriteria(context => context.LatestCommitHasVersionTag(), "The latest commit doesn't have a version tag")
    .WithCriteria(IsRunningOnWindows(), "The GitHub Package Repository only supports the NuGet client on Windows")
    .IsDependentOn("Package-NuGet")
    .Does<PackageMetadata>(package =>
{
    var settings = new NuGetSourcesSettings
    {
        UserName = EnvironmentVariable("GitHubUser"),
        Password = EnvironmentVariable("GitHubPassword"),
        IsSensitiveSource = true
    };

    if (!NuGetHasSource(Urls.GitHubNuGetPackageRegistryUrl.AbsoluteUri, settings))
    {
        NuGetAddSource(
            "ecampidoglio.github.com",
            Urls.GitHubNuGetPackageRegistryUrl.AbsoluteUri,
            settings);
    }

    NuGetPush(
        package.FullPath,
        new NuGetPushSettings
        {
            Source = "ecampidoglio.github.com",
            SkipDuplicate = true
        });
})
.Finally(() =>
{
    NuGetRemoveSource(
        "ecampidoglio.github.com",
        Urls.GitHubNuGetPackageRegistryUrl.AbsoluteUri);
});

Task("Publish-Docker-Image-To-GitHub")
    .WithCriteria(context => context.LatestCommitHasVersionTag(), "The latest commit doesn't have a version tag")
    .IsDependentOn("Package-Docker")
    .Does<ImageMetadata>(image =>
{
    var originalName = image.Name;
    image.Registry =  Urls.GitHubDockerPackageRegistryUrl;
    image.Repository = "ecampidoglio/linker/linker";

    DockerTag(originalName, image.Name);

    DockerLogin(
        EnvironmentVariable("GitHubUser"),
        EnvironmentVariable("GitHubPassword"),
        Urls.GitHubDockerPackageRegistryUrl);

    DockerPush(image.Name);
})
.Finally(() =>
{
    DockerLogout(Urls.GitHubDockerPackageRegistryUrl);
});

Task("Set-Build-Number")
    .WithCriteria(() => !BuildSystem.IsLocalBuild)
    .IsDependentOn("Version")
    .Does<PackageMetadata>(package =>
{
    if (BuildSystem.IsRunningOnAzurePipelinesHosted)
    {
        TFBuild.Commands.UpdateBuildNumber(
            $"{package.Version}+{TFBuild.Environment.Build.Number}");
    }
    else if (BuildSystem.IsRunningOnAppVeyor)
    {
        AppVeyor.UpdateBuildVersion(
            $"{package.Version}+{AppVeyor.Environment.Build.Number}");
    }
    else if (BuildSystem.IsRunningOnTeamCity)
    {
        TeamCity.SetBuildNumber(
            $"{package.Version}+{TeamCity.Environment.Build.Number}");
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

Task("Publish-Packages")
    .IsDependentOn("Publish-NuGet-Package-To-GitHub")
    .IsDependentOn("Publish-Docker-Image-To-GitHub");

Task("Build")
    .IsDependentOn("Compile")
    .IsDependentOn("Test")
    .IsDependentOn("Build-Frontend");

Task("Build-CI")
    .IsDependentOn("Clean")
    .IsDependentOn("Compile")
    .IsDependentOn("Test")
    .IsDependentOn("Version")
    .IsDependentOn("Build-Frontend")
    .IsDependentOn("Package-Zip")
    .IsDependentOn("Publish-Test-Results")
    .IsDependentOn("Publish-Artifacts")
    .IsDependentOn("Set-Build-Number");

Task("Deploy-CI")
    .IsDependentOn("Clean")
    .IsDependentOn("Deploy-Kudu")
    .IsDependentOn("Publish-Test-Results")
    .IsDependentOn("Publish-Artifacts")
    .IsDependentOn("Publish-Packages")
    .IsDependentOn("Set-Build-Number");

RunTarget(target);
