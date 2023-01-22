using Cellarium.Api;
using CellariumDaemon.Logic;
using YandexDisk.Client.Http;

namespace CellariumDaemon;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public Worker(IConfiguration configuration, ILoggerFactory factory)
    {
        _configuration = configuration;
        _loggerFactory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var wd = new WatchDog(
            new YandexCloudApi(new DiskHttpApi(""), clear: false), _configuration, _loggerFactory.CreateLogger<WatchDog>());
        wd.Run();
    }
}