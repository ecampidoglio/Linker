#load paths.cake

public static string ReadVersionFromProjectFile(ICakeContext context)
{
    var versionNode = "/Project/PropertyGroup/Version/text()";
    return context.XmlPeek(Paths.ProjectFile, versionNode);
}
