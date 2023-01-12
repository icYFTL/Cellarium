using Cellarium.Api;
using Cellarium.Handlers;
using CellariumDaemon.Logic;
using YandexDisk.Client.Http;

namespace CellariumDaemon;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var wd = new WatchDog("",
            new YandexCloudApi(new DiskHttpApi("")));
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     await Task.Delay(1000, stoppingToken);
        // }
    }
}