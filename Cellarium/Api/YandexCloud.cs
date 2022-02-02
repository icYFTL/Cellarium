using YandexDisk.Client;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;
using Cellarium.Models;

namespace Cellarium.Api
{
    public class YandexCloudApi
    {
        private readonly IDiskApi _yandex;
        private readonly bool _clear;

        public YandexCloudApi(IDiskApi yandex, bool clear = true)
        {
            _yandex = yandex;
            _clear = clear;
        }

        private void OnSuccess(string path, string taskType, string fileType) => Console.WriteLine($"[Success] File({fileType}): {path}");
        private void OnError(string path,string taskType, string fileType, string ex) => Console.WriteLine($"[Error] File({fileType}): {path}\n{ex}");

        public async Task<bool> UploadFileAsync(CellariumFile file, bool overwrite = false, Action<String, String, String>? onSuccess = null, Action<String, String, String, String>? onError = null)
        {
            onSuccess ??= OnSuccess;
            onError ??= OnError;
            
            Console.WriteLine($"[Info] File: {file.FileName} in progress");
            
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

                onSuccess(file.InternalPath,  "Upload", "Internal");
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
            var resourceDescription = await _yandex.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = externalPath
            }, CancellationToken.None);

            return resourceDescription.Embedded.Items.Where(item => item.Type == ResourceType.File).AsQueryable();
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

            return (await Task.WhenAll(result).ConfigureAwait(false)).Select(x => (MemoryStream) x).ToList();
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

        public async Task<bool> CreatePathAsync(string externalPath)
        {
            try
            {
                await _yandex.Commands.CreateDictionaryAsync(externalPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveFileAsync(string externalPath, bool deletePermanently = false, Action<String, String, String>? onSuccess = null, Action<String, String, String, String>? onError = null)
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
}