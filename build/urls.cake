public static class Urls
{
    public static Uri WebDeployUrl => new Uri("linker-demo.scm.azurewebsites.net:443");
    public static Uri KuduDeployUrl => new Uri("https://linker-demo.scm.azurewebsites.net/api/zipdeploy");
    public static Uri OctopusDeployServerUrl => new Uri("http://octopus-megakemp.northeurope.cloudapp.azure.com");
    public static Uri GitHubNuGetPackageRegistryUrl => new Uri("https://nuget.pkg.github.com/ecampidoglio/index.json");
    public static string GitHubDockerPackageRegistryUrl => "docker.pkg.github.com";
    public static string DockerRegistryUrl => "linkerdemo.azurecr.io";
}
