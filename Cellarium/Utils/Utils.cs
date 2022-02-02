using System.Diagnostics;
using System.Reflection;

namespace Cellarium.Utils;

public static class Utils
{
    public static string GetAssemblyVersion()
    {
        return Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
            .InformationalVersion;
    }
}