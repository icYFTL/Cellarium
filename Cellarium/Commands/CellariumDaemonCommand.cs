using Cellarium.Api;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using Cellarium.Utils;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;

public class CellariumDaemonCommand : BaseCommand
{
    public override void Run(params BaseParameter [] arguments)
    {
        base.Run(arguments);
        
        var daemonHandler = new DaemonHandler(new YandexCloudApi(new DiskHttpApi(Environment.GetEnvironmentVariable("token"))));

        var status = arguments.FirstOrDefault(x => x.Content == "status");
        var enable = arguments.FirstOrDefault(x => x.Content == "enable");
        var disable = arguments.FirstOrDefault(x => x.Content == "disable");
        var restart = arguments.FirstOrDefault(x => x.Content == "restart");
        var internalBasePath = arguments.FirstOrDefault(x => x.Content == "internal_base_path");
        var externalBasePath = arguments.FirstOrDefault(x => x.Content == "external_base_path");
        
        if (status is not null)
        {  
            Logger.Info( $"Current status is: {daemonHandler.GetStatus()}");
            return;
        }
        if (enable is not null)
        {
            daemonHandler.Enable();
            var cstatus = daemonHandler.GetStatus() == "active";
            if (cstatus)
            {
                Logger.Info($"Daemon was started");
                return;
            }
            Logger.Warn($"Can\'t enable daemon. Current status is {status}");
            return;
        }
        if (disable is not null)
        {
            daemonHandler.Disable();
            var cstatus = daemonHandler.GetStatus() != "active";
            if (cstatus)
            {
                Logger.Info($"Daemon was stopped");
                return;
            }
            Logger.Warn($"Can\'t disable daemon. Current status is {status}");
            return;
        }
        if (restart is not null)
        {
            
        }
        if (internalBasePath is not null)
        {
            daemonHandler.SetInternalBasePath(internalBasePath.Value);
            return;
        }
        if (externalBasePath is not null)
        {
            daemonHandler.SetExternalBasePath(externalBasePath.Value);
            return;
        }
        
        Logger.Info(FullDescription);
    }
    
    public CellariumDaemonCommand()
    {
        Description = "Cellarium Daemon settings";
        FullDescription = @"Cellarium Daemon settings:
status - Daemon's status
enable - Enable daemon
disable - Disable daemon
restart - Restart daemon
internal_base_path - Daemon's watch path
external_base_path - External path on YD";

        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "d"
            },
            new()
            {
                Type = AliasTypeEnum.Fully,
                Content = "daemon"
            }
        };
        Parameters = new List<BaseParameter>
        {
            new()
            {
                Content = "status",
                Value = null,
                Optional = true
            },
            new()
            {
                Content = "register",
                Value = null,
                Optional = true
            },
            new()
            {
                Content = "unregister",
                Value = null,
                Optional = true
            },
            new()
            {
                Content = "enable",
                Value = null,
                Optional = true
            },
            new()
            {
                Content = "disable",
                Value = null,
                Optional = true
            },
            new()
            {
                Content = "restart",
                Value = null,
                Optional = true
            },
            new()
            {
                Content = "internal_base_path",
                Value = "/etc/test",
                Optional = true
            },
            new()
            {
                Content = "external_base_path",
                Value = "/test",
                Optional = true
            }
        };
        Logger = new Logger().GetLogger<CellariumDaemonCommand>();
    }
}