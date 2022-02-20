using Cellarium.Api;
using Cellarium.Models;
using NLog;

namespace Cellarium.Handlers;
using Logger=Cellarium.Utils.Logger;

public class IoHandler
{
    private readonly string _externalPath;
    private readonly YandexCloudApi _yandex;
    private readonly bool _forcePathCreate;
    private readonly ILogger _logger;
    
    public IoHandler(YandexCloudApi yandex,  string externalPath, bool forcePathCreate = false)
    {
        _externalPath = externalPath;
        _yandex = yandex;
        _forcePathCreate = forcePathCreate;
        _logger = new Logger().GetLogger<IoHandler>();
        CreateExternalPath();
    }

    private async void CreateExternalPath()
    {
        if (!_yandex.IsPathExistsAsync(_externalPath).Result)
        {
            if (_forcePathCreate)
                await _yandex.CreatePathAsync(_externalPath);
            else
            {
                _logger.Fatal($"External path {_externalPath} does not exist");
                throw new InvalidOperationException();
            }
        }
    }

    private IEnumerable<FileInfo> GetLocalFiles(string internalPath)
    {
        IEnumerable<FileInfo> files = new List<FileInfo>();
        try
        {
            files = Directory.EnumerateFiles(internalPath, "*.*", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x));
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex.ToString());
            throw;
        }

        return
            files; //.Where(x => !LocalStorage.Contains(item => item.Path == x.FullName || (item.DoNotTouchUntil ?? DateTime.Now) > DateTime.Now));
    }

    public void TransferToCloud(string internalPath, bool overwrite = false)
    {
        var internal_root = internalPath.Split(Path.DirectorySeparatorChar).TakeLast(1).First();
        var files = GetLocalFiles(internalPath) ?? throw new ArgumentNullException("GetLocalFiles()");
        var tasks = new List<Task>();
        foreach (var file in files)
        {
            var backupProjectRoot =
                file.FullName.Split(Path.DirectorySeparatorChar).SkipWhile(x => x != internal_root).Skip(1).First();

            tasks.Add(Task.Run(async () =>
            {
                await _yandex.UploadFileAsync(new CellariumFile
                {
                    InternalPath = file.FullName,
                    ExternalPath = Path.Combine(_externalPath, backupProjectRoot),
                    FileName = file.Name
                }, overwrite);
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }


    // public void TransferToCloud(object source, FileSystemEventArgs e)
    // {
    //     var tasks = new List<Task>();
    //
    //     var backupProjectRoot =
    //         e.FullPath.Split(Path.DirectorySeparatorChar).SkipWhile(x => x != _internalRoot).Skip(1).First();
    //
    //     tasks.Add(Task.Run(() =>
    //     {
    //         _yandex.UploadFileAsync(new CellariumFile
    //         {
    //             InternalPath = e.FullPath,
    //             ExternalPath = Path.Combine(_externalPath, backupProjectRoot),
    //             FileName = e.Name ?? $"{DateTimeOffset.Now.ToUnixTimeSeconds()}.undefined"
    //         });
    //     }));
    //
    //
    //     Task.WaitAll(tasks.ToArray());
    // }


    public void ClearExpired(int delta, bool deletePermanently = false)
    {
        var expiredFiles = _yandex.GetFileListAsync(_externalPath).Result
            .Where(file => file.Created + TimeSpan.FromDays(delta) < DateTime.Now);
        var tasks = new List<Task>();

        foreach (var file in expiredFiles)
        {
            tasks.Add(Task.Run(async () =>
            {
                await _yandex.RemoveFileAsync(file.Path, deletePermanently);
            }));
        }
        
        Task.WaitAll(tasks.ToArray()); 
    }
    
    public void ClearCount(int maxCount, bool deletePermanently = false)
    {
        var files = _yandex.GetFileListAsync(_externalPath).Result.OrderByDescending(x => x.Created);
        var filesToDelete = files.Take(new Range(maxCount, files.Count()));
        var tasks = new List<Task>();
         
        foreach (var file in filesToDelete)
        {
            tasks.Add(Task.Run(async () =>
            {
                await _yandex.RemoveFileAsync(file.Path, deletePermanently);
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
    }
}