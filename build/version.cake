#addin nuget:?package=Cake.Git&version=0.19.0

#load paths.cake

using System.Text.RegularExpressions;

public static string ReadVersionFromProjectFile(ICakeContext context)
{
    var versionNode = "/Project/PropertyGroup/Version/text()";

    return context.XmlPeek(
        Paths.ProjectFile,
        versionNode,
        new XmlPeekSettings
        {
            SuppressWarning = true
        });
}

public static bool LatestCommitHasVersionTag(this ICakeContext context)
{
    var latestTag = context.GitDescribe(Paths.RepoDirectory);
    var isVersionTag = Regex.IsMatch(latestTag, @"v[0-9]*");
    var noCommitsAfterLatestTag = !Regex.IsMatch(latestTag, @"\-[0-9]+\-");

    return isVersionTag && noCommitsAfterLatestTag;
}
