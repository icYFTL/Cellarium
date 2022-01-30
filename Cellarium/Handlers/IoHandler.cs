using Cellarium.Api;
using Cellarium.Models;

namespace Cellarium.Handlers;

public class IoHandler
{
    private readonly string _internalPath;
    private readonly string _internalRoot;
    private readonly string _externalPath;
    private readonly YandexCloudApi _yandex;
    private readonly bool _forcePathCreate;
    private readonly bool _overwrite;

    public IoHandler(YandexCloudApi yandex, string internalPath, string externalPath, bool forcePathCreate = false,
        bool overwrite = false)
    {
        _internalPath = internalPath;
        _internalRoot = _internalPath.Split(Path.DirectorySeparatorChar).TakeLast(1).First();
        _externalPath = externalPath;
        _yandex = yandex;
        _forcePathCreate = forcePathCreate;
        _overwrite = overwrite;
        CreateExternalPath();
    }

    private async void CreateExternalPath()
    {
        if (!_yandex.IsPathExistsAsync(_externalPath).Result)
        {
            if (_forcePathCreate)
                await _yandex.CreatePathAsync(_externalPath);
            else
                throw new InvalidOperationException($"External path {_externalPath} does not exist");
        }
    }

    private IEnumerable<FileInfo> GetLocalFiles()
    {
        IEnumerable<FileInfo> files = new List<FileInfo>();
        try
        {
            files = Directory.EnumerateFiles(_internalPath, "*.*", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x));
        }
        catch (Exception ex)
        {
            // _logger.LogCritical(ex.ToString());
            throw;
        }

        return
            files; //.Where(x => !LocalStorage.Contains(item => item.Path == x.FullName || (item.DoNotTouchUntil ?? DateTime.Now) > DateTime.Now));
    }

    public void TransferToCloud()
    {
        var files = GetLocalFiles() ?? throw new ArgumentNullException("GetLocalFiles()");
        var tasks = new List<Task>();
        foreach (var file in files)
        {
            var backupProjectRoot =
                file.FullName.Split(Path.DirectorySeparatorChar).SkipWhile(x => x != _internalRoot).Skip(1).First();

            tasks.Add(Task.Run(async () =>
            {
                await _yandex.UploadFileAsync(new CellariumFile
                {
                    InternalPath = file.FullName,
                    ExternalPath = Path.Combine(_externalPath, backupProjectRoot),
                    FileName = file.Name
                }, _overwrite);
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }


    public void TransferToCloud(object source, FileSystemEventArgs e)
    {
        var tasks = new List<Task>();

        var backupProjectRoot =
            e.FullPath.Split(Path.DirectorySeparatorChar).SkipWhile(x => x != _internalRoot).Skip(1).First();

        tasks.Add(Task.Run(() =>
        {
            _yandex.UploadFileAsync(new CellariumFile
            {
                InternalPath = e.FullPath,
                ExternalPath = Path.Combine(_externalPath, backupProjectRoot),
                FileName = e.Name ?? $"{DateTimeOffset.Now.ToUnixTimeSeconds()}.undefined"
            });
        }));


        Task.WaitAll(tasks.ToArray());
    }


    private void ClearExpired(object source, FileSystemEventArgs e)
    {
        throw new NotImplementedException();
        // var expiredFiles = _yandex.GetFileListAsync(_externalPath).Result
        //     .Where(file => file.);
        //
        // foreach (var file in expiredFiles)
        // {
        //     try
        //     {
        //         _yandex.RemoveFileAsync(file.Path);
        //         // _logger.LogInformation($"[External Removing]\nStatus: true; Path: {file.Path}");
        //     }
        //     catch (Exception ex)
        //     {
        //         // _logger.LogError($"[External Removing]\nStatus: false; Path: {file.Path}\n{ex}");
        //     }
        // }
    }
}