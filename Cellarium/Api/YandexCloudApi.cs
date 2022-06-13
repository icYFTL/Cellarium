using YandexDisk.Client;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;
using Cellarium.Models;
using NLog;


namespace Cellarium.Api;

using Logger = Utils.Logger;

public class YandexCloudApi
{
    private readonly IDiskApi _yandex;
    private readonly bool _clear;
    private readonly ILogger _logger;

    public YandexCloudApi(IDiskApi yandex, bool clear = true)
    {
        _yandex = yandex;
        _clear = clear;
        _logger = new Logger().GetLogger<YandexCloudApi>();
    }

    private void OnSuccess(string path, string taskType, string fileType) =>
        _logger.Info($"File({fileType}) sent: {path}");

    private void OnError(string path, string taskType, string fileType, string ex) =>
        _logger.Error($"File({fileType}): {path}\n{ex}");

    public async Task<bool> UploadFileAsync(CellariumFile file, bool overwrite = false, bool forceCreateExternalPath = false,
        Action<String, String, String>? onSuccess = null, Action<String, String, String, String>? onError = null)
    {
        onSuccess ??= OnSuccess;
        onError ??= OnError;

        _logger.Info($"File: {file.FileName} in progress");

        // _logger.LogDebug($"[Uploading]\nStatus: Started; Path: {file.InternalPath}");
        try
        {
            if (!IsPathExistsAsync(file.ExternalPath).Result)
                await CreatePathAsync(file.ExternalPath);

            await _yandex.Files.UploadFileAsync(file.FullExternalPath, overwrite, file.InternalPath,
                CancellationToken.None);

            // _logger.LogDebug($"[Uploading]\nStatus: Done; Path: {file.InternalPath}");
            // LocalStorage.Remove(x => x.Path == file.InternalPath);
            if (_clear)
                File.Delete(file.InternalPath);

            onSuccess(file.InternalPath, "Upload", "Internal");
            return true;
        }
        catch (Exception ex)
        {
            onError(file.InternalPath, "Upload", "Internal", ex.ToString());
            // _logger.LogError($"[Uploading]\nStatus: Exception; Path: {file.InternalPath}\n{ex}");
            // LocalStorage.Get().First(x => x.Path == file.InternalPath).DoNotTouchUntil = DateTime.Now + TimeSpan.FromHours(1);
            // await Task.FromException(ex);
            return false;
        }
    }

    public async Task<IQueryable<Resource>> GetFileListAsync(string externalPath)
    {
        var items = new List<Resource>();
        var offset = 0;

        while (true)
        {
            var _items = await _yandex.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = externalPath,
                Limit = 100,
                Offset = offset
            }, CancellationToken.None);

            if (!_items.Embedded.Items.Any())
                break;

            items.AddRange(_items.Embedded.Items);
            offset += 100;
        }

        return items.Where(item => item.Type == ResourceType.File).AsQueryable();
    }

    public async Task DownloadFilesAsync(IQueryable<Resource> files, string externalPath, string internalPath)
    {
        var result = files.Select(
            file => _yandex.Files.DownloadFileAsync(externalPath, Path.Combine(internalPath, file.Name),
                CancellationToken.None));

        await Task.WhenAll(result);
    }

    public async Task<List<MemoryStream>> DownloadFilesAsync(IQueryable<Resource> files, string externalPath)
    {
        var result = files.Select(
            file => _yandex.Files.DownloadFileAsync(externalPath,
                CancellationToken.None));

        return (await Task.WhenAll(result).ConfigureAwait(false)).Select(x => (MemoryStream)x).ToList();
    }

    public async Task<bool> IsPathExistsAsync(string externalPath)
    {
        try
        {
            await _yandex.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = externalPath
            });
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private async Task<bool> CreatePathAsync(string externalPath)
    {
        try
        {
            var pathSegments = externalPath.Split("/").Skip(1).ToArray();
            var sub = $"/{pathSegments[0]}";
            try
            {
                await _yandex.Commands.CreateDictionaryAsync(sub);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("existent")) // Idk how to handle it properly
                    throw;
            }

            for (var i = 1; i < pathSegments.Length; ++i)  // Cause we don't have method to create full path...
            {
                sub += $"/{pathSegments[i]}";
                try
                {
                    await _yandex.Commands.CreateDictionaryAsync(sub);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("existent")) // Idk how to handle it properly
                        throw;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveFileAsync(string externalPath, bool deletePermanently = false,
        Action<String, String, String>? onSuccess = null, Action<String, String, String, String>? onError = null)
    {
        onSuccess ??= OnSuccess;
        onError ??= OnError;

        try
        {
            await _yandex.Commands.DeleteAsync(new DeleteFileRequest
            {
                Path = externalPath,
                Permanently = deletePermanently
            });
            onSuccess(externalPath, "Delete", "External");
            return true;
        }
        catch (Exception ex)
        {
            onError(externalPath, "Delete", "External", ex.ToString());
            return false;
        }
    }
}