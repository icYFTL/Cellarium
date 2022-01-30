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
        private readonly bool _deletePermanently;

        public YandexCloudApi(IDiskApi yandex, bool clear = true, bool deletePermanently = false)
        {
            _yandex = yandex;
            _clear = clear;
            _deletePermanently = deletePermanently;
        }

        private void OnSuccess(CellariumFile file) => Console.WriteLine($"[Success] File: {file.InternalPath}");
        private void OnError(CellariumFile file, string ex) => Console.WriteLine($"[Error] File: {file.InternalPath}\n{ex}");

        public async Task<bool> UploadFileAsync(CellariumFile file, bool overwrite = false, Action<CellariumFile>? onSuccess = null, Action<CellariumFile, String>? onError = null)
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

                onSuccess(file);
                return true;
            }
            catch (Exception ex)
            {
                onError(file, ex.ToString());
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

        public async Task<Link> CreatePathAsync(string externalPath)
        {
            return await _yandex.Commands.CreateDictionaryAsync(externalPath); // Idk why dictionary xd
        }

        public async Task RemoveFileAsync(string externalPath)
        {
            _yandex.Commands.DeleteAsync(new DeleteFileRequest
            {
                Path = externalPath,
                Permanently = _deletePermanently
            });
        }
    }
}