using Cellarium.Api;
using Cellarium.Handlers;
using Cellarium.Models;

namespace CellariumDaemon.Logic;

public class WatchDog
{
    private readonly IoHandler _ioHandler;
    private readonly YandexCloudApi _yapi;
    private readonly string _path;
    private string _rootDir;

    private string rootDir
    {
        get
        {
            if (String.IsNullOrEmpty(_rootDir))
            {
                _rootDir = new DirectoryInfo(_path).Name;
            }

            return _rootDir;
        }
    }


    public WatchDog(string path, YandexCloudApi yapi)
    {
        // TODO: Check path existence
        _yapi = yapi;

        var watcher = new FileSystemWatcher();
        watcher.Path = path;
        _path = path;

        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                        | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        // watcher.Filter = "*.txt";

        watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.Created += new FileSystemEventHandler(OnCreated);
        watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        watcher.Renamed += new RenamedEventHandler(OnRenamed);

        watcher.EnableRaisingEvents = true;
    }

    private string CatOffPath(string sourcePath, string catFrom, bool withLeadingSlash = true)
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

        return result;
    }

    private async void OnDeleted(object source, FileSystemEventArgs e)
    {
        _yapi.RemoveFileAsync(CatOffPath(e.FullPath, rootDir));
    }

    private async void OnCreated(object source, FileSystemEventArgs e)
    {
        var path = CatOffPath(e.FullPath, rootDir);
        _yapi.UploadFileAsync(new CellariumFile
        {
            ExternalPath = path,
            FileName = e.Name,
            InternalPath = e.FullPath
        }, forceCreateExternalPath:true, overwrite:true);
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
    }

    private void OnRenamed(object source, RenamedEventArgs e)
    {
        Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
    }
}