#load paths.cake

using System.Text.RegularExpressions;

public static string ReadVersionFromProjectFile(ICakeContext context)
{
    var versionNode = "/Project/PropertyGroup/Version/text()";

    // TODO: return the content of the version node in the project file
    return null;
}
