namespace CellariumDaemon.Utils;

public static class Utils
{
    public static string CatOffPathToFile(string sourcePath, string catFrom, bool withLeadingSlash = true)
    {
        var index = sourcePath.IndexOf(catFrom, StringComparison.Ordinal);
        var result = sourcePath.Substring(index);

        if (!withLeadingSlash)
        {
            if (result[0] == '/')
                result = result.Substring(1);
        }
        else if (result[0] != '/')
            result = result.Insert(0, "/");

        return result.Replace("\\", "/");
    }

    public static string CatOffPathToDir(string sourcePath, string catFrom, bool withLeadingSlash = true)
    {
        var index = sourcePath.IndexOf(catFrom, StringComparison.Ordinal);
        var result = sourcePath.Substring(index);

        if (!withLeadingSlash)
        {
            if (result[0] == '/')
                result = result.Substring(1);
        }
        else if (result[0] != '/')
            result = result.Insert(0, "/");

        return Path.GetDirectoryName(result);
    }
}