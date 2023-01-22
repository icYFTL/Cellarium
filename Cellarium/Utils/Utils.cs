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

    public static string? RunCommandWithOutput(string command)
    {
        string filename;
        string args;

        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.MacOSX:
            case PlatformID.Unix:
                filename = command;
                break;
            case PlatformID.Win32Windows:
                filename = "cmd.exe";
                args = $"/C {command}";
                break;
            default:
                throw new NotSupportedException($"OS {Environment.OSVersion.Platform} is not supported");
        }

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
        catch
        {
            return null;
        }
    }
}