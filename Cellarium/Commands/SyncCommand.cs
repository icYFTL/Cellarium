using System.Reflection;
using System.Text;
using Cellarium.Api;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;
using Utils = Utils.Utils;

public class SyncCommand : BaseCommand
{
    public sealed override string Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }
    
    public override void Run(params BaseParameter [] arguments)
    {
        base.Run(arguments);
        
        var internalPath = arguments.First(x => x.Content == "internal_path").Value;
        var externalPath = arguments.First(x => x.Content == "external_path").Value;
        
        var forceCreateExternalPath = false;
        bool.TryParse(arguments.FirstOrDefault(x => x.Content == "force_create_external_path")?.Value, out forceCreateExternalPath);
        
        var clear = true;
        bool.TryParse(arguments.FirstOrDefault(x => x.Content == "clear")?.Value, out clear);
        
        var removePermanently = false;
        bool.TryParse(arguments.FirstOrDefault(x => x.Content == "remove_permanently_external")?.Value, out removePermanently);
        
        var overwrite = false;
        bool.TryParse(arguments.FirstOrDefault(x => x.Content == "overwrite")?.Value, out overwrite);
        

        var yandexCloudApi = new YandexCloudApi(new DiskHttpApi(Environment.GetEnvironmentVariable("token")), clear, removePermanently);
        var handler = new IoHandler(yandexCloudApi, internalPath, externalPath, forceCreateExternalPath, overwrite);
        handler.TransferToCloud();
    }

    public SyncCommand()
    {
        Description = "Sync specified directory with yandex drive";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "s"
            },
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "sync"
            }
        };
        Parameters = new List<BaseParameter>
        {
            new ()
            {
                Content = "internal_path",
                Value = "/dev/null",
                Optional = false
            },
            new ()
            {
                Content = "external_path",
                Value = "/",
                Optional = false
            },
            new ()
            {
                Content = "force_create_external_path",
                Value = "true"
            },
            new ()
            {
                Content = "clear",
                Value = "true"
            },
            new ()
            {
                Content = "remove_permanently_external",
                Value = "false"
            },
            new ()
            {
                Content = "overwrite",
                Value = "false"
            }
        };
    }
}