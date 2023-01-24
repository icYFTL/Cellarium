using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;
using Cellarium.Api;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using Logger = Cellarium.Utils.Logger;

namespace Cellarium.Handlers;

public class DaemonHandler
{
    private readonly IConfiguration _daemonConfiguration;
    private readonly string _daemonConfigPath;
    private readonly ILogger _logger;
    private readonly YandexCloudApi _yandexCloudApi;

    public DaemonHandler(YandexCloudApi yandexCloudApi)
    {
        if (!(Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX))
        {
            throw new NotSupportedException($"Cellarium Daemon is not supported on {Environment.OSVersion.Platform} yet.");
        }

        if (!IsRegistered())
        {
            throw new CheckoutException(
                "Cellarium daemon is not registered. Do it manually or re-install the cellarium.");
        }

        _yandexCloudApi = yandexCloudApi;

#if DEBUG
        _daemonConfigPath = Path.Join(Environment.CurrentDirectory, "..", "..", "..", "..", "CellariumDaemon",
            "appsettings.json");
#else
        _daemonConfigPath = "/usr/share/cellarium/daemon/appsettings.json";
#endif

        _daemonConfiguration = new ConfigurationBuilder()
            .AddJsonFile(_daemonConfigPath)
            .Build();

        _logger = new Logger().GetLogger<DaemonHandler>();
    }

    public bool IsRegistered()
    {
        if (!File.Exists("/etc/systemd/system/cellarium_daemon.service"))
            return false;

        var result = Utils.Utils.RunCommandWithOutput("systemctl", "status cellarium_daemon");

        return !(result ?? "could not be found").Contains("could not be found");
    }

    public string? GetStatus()
    {
        var result = Utils.Utils.RunCommandWithOutput("systemctl", "status cellarium_daemon");
        if (result is null)
            throw new SystemException("Something went wrong with cellarium_daemon.");

        var match = Regex.Match(result, "Active:\\s(\\w+)");

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }

    public void Restart()
    {
        var result = Utils.Utils.RunCommandWithOutput("systemctl", "stop cellarium_daemon");
        if (result is null)
            throw new SystemException("Something went wrong with cellarium_daemon.");
        
        _daemonConfiguration["Token"] = Environment.GetEnvironmentVariable("token");
        _syncDaemonConfig();
        
        result = Utils.Utils.RunCommandWithOutput("systemctl", "start cellarium_daemon");
        if (result is null)
            throw new SystemException("Something went wrong with cellarium_daemon.");
    }
    
    public void Enable()
    {
        _daemonConfiguration["Token"] = Environment.GetEnvironmentVariable("token");
        _syncDaemonConfig();
        
        var result = Utils.Utils.RunCommandWithOutput("systemctl", "start cellarium_daemon");
        if (result is null)
            throw new SystemException("Something went wrong with cellarium_daemon.");
    }

    public void Disable()
    {
        var result = Utils.Utils.RunCommandWithOutput("systemctl", "stop cellarium_daemon");
        if (result is null)
            throw new SystemException("Something went wrong with cellarium_daemon.");
    }

    public void SetInternalBasePath(string path)
    {
        if (!Directory.Exists(path))
        {
            _logger.Error($"Path {path} is not exists");
            return;
        }

        _daemonConfiguration["InternalBasePath"] = path;
        _syncDaemonConfig();
    }

    public void SetExternalBasePath(string path)
    {
        if (!_yandexCloudApi.IsPathExistsAsync(path).Result)
        {
            _logger.Error($"Path {path} is not exists");
            return;
        }

        _daemonConfiguration["ExternalBasePath"] = path;
        _syncDaemonConfig();
    }

    private void _syncDaemonConfig()
    {
        var configDictionary = _daemonConfiguration.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
        string json = JsonConvert.SerializeObject(configDictionary);

        File.WriteAllText(_daemonConfigPath, json, Encoding.UTF8);
    }
}