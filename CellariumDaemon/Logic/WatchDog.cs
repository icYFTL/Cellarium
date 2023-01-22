using Cellarium.Api;
using Cellarium.Models;

namespace CellariumDaemon.Logic;

public class WatchDog
{
    private readonly YandexCloudApi _yapi;
    private readonly string _baseInternalPath;
    private readonly string _baseExternalPath;
    private string _rootDir = String.Empty;
    private readonly ILogger<WatchDog> _logger;

    private string RootDir
    {
        get
        {
            if (String.IsNullOrEmpty(_rootDir))
            {
                _rootDir = new DirectoryInfo(_baseInternalPath).Name;
            }

            return _rootDir;
        }
    }


    public WatchDog(YandexCloudApi yapi, IConfiguration configuration, ILogger<WatchDog> logger)
    {
        _yapi = yapi;
        _baseInternalPath = configuration["InternalBasePath"];
        _baseExternalPath = configuration["ExternalBasePath"];
        _logger = logger;

        if (!Directory.Exists(_baseInternalPath))
        {
            _logger.LogError($"{_baseInternalPath} -> not a valid path");
            throw new ArgumentException("Not a valid base internal path");
        }
    }

    public void Run()
    {
        var watcher = new FileSystemWatcher();
        watcher.Path = _baseInternalPath;

        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                        | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        // watcher.Filter = "*.txt";

        watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.Created += new FileSystemEventHandler(OnCreated);
        watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        watcher.Renamed += new RenamedEventHandler(OnRenamed);

        watcher.EnableRaisingEvents = true;
        watcher.IncludeSubdirectories = true;
    }

    private async void OnDeleted(object source, FileSystemEventArgs e)
    {
        _yapi.RemoveFileAsync(Path.Join(_baseExternalPath, Utils.Utils.CatOffPathToFile(e.FullPath, RootDir)));
    }

    private async void OnCreated(object source, FileSystemEventArgs e)
    {
        var path = Path.Join(_baseExternalPath, Utils.Utils.CatOffPathToDir(e.FullPath, RootDir));
        _yapi.UploadFileAsync(new CellariumFile
        {
            ExternalDir = path,
            FileName = Path.GetFileName(e.Name),
            InternalPath = e.FullPath
        }, forceCreateExternalPath: true, overwrite: true);
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        var path = Path.Join(_baseExternalPath, Utils.Utils.CatOffPathToDir(e.FullPath, RootDir));
        _yapi.UploadFileAsync(new CellariumFile
        {
            ExternalDir = path,
            FileName = Path.GetFileName(e.Name),
            InternalPath = e.FullPath
        }, forceCreateExternalPath: true, overwrite: true);
    }

    // TODO:
    private void OnRenamed(object source, RenamedEventArgs e)
    {
        var path = Path.Join(_baseExternalPath, Utils.Utils.CatOffPathToDir(e.FullPath, RootDir));
        _yapi.UploadFileAsync(new CellariumFile
        {
            ExternalDir = path,
            FileName = Path.GetFileName(e.Name),
            InternalPath = e.FullPath
        }, forceCreateExternalPath: true, overwrite: true);

        _yapi.RemoveFileAsync(Path.Join(_baseExternalPath, Utils.Utils.CatOffPathToFile(e.OldFullPath, RootDir)));
    }
}