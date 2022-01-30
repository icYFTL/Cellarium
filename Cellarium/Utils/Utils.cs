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
    
    public static Process? PriorProcess()
        // Returns a System.Diagnostics.Process pointing to
        // a pre-existing process with the same name as the
        // current one, if any; or null if the current process
        // is unique.
    {
        Process curr = Process.GetCurrentProcess();
        Process[] procs = Process.GetProcessesByName(curr.ProcessName);
        foreach (Process p in procs)
        {
            if ((p.Id != curr.Id) &&
                (p.MainModule.FileName == curr.MainModule.FileName))
                return p;
        }
        return null;
    }
}