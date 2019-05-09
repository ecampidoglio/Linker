#load paths.cake

public static string ReadVersionNumberFromProject(ICakeContext context)
{
    var versionNode = "/Project/PropertyGroup/Version/text()";
    return context.XmlPeek(Paths.ProjectFile, versionNode);
}
