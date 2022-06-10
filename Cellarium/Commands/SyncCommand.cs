using System.Reflection;
using System.Text;
using Cellarium.Api;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;

public class SyncCommand : BaseCommand
{
    public sealed override string? Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }
    
    public override void Run(params BaseParameter [] arguments)
    {
        base.Run(arguments);
        
        var internalPath = arguments.First(x => x.Content == "internal_path").Value;
        var externalPath = arguments.First(x => x.Content == "external_path").Value;
        
        var forceCreateExternalPath = false;
        var _fcep = arguments.FirstOrDefault(x => x.Content == "force_create_external_path")?.Value;
        if (!String.IsNullOrEmpty(_fcep))
            if (!bool.TryParse(_fcep, out forceCreateExternalPath))
                throw new ArgumentException("Bad force_create_external_path passed");
        
        
        var clear = true;
        var _c = arguments.FirstOrDefault(x => x.Content == "clear")?.Value;
        if (!String.IsNullOrEmpty(_c))
            if (!bool.TryParse(_c, out clear))
                throw new ArgumentException("Bad clear passed");
        

        var overwrite = false;
        var _o = arguments.FirstOrDefault(x => x.Content == "overwrite")?.Value;
        if (!String.IsNullOrEmpty(_o))
            if (!bool.TryParse(_o, out overwrite))
                throw new ArgumentException("Bad overwrite passed");
        
        var yandexCloudApi = new YandexCloudApi(new DiskHttpApi(Environment.GetEnvironmentVariable("token")), clear);
        var handler = new IoHandler(yandexCloudApi, externalPath, forceCreateExternalPath);
        handler.TransferToCloud(internalPath, overwrite);
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
                Value = "false"
            },
            new ()
            {
                Content = "clear",
                Value = "true"
            },
            new ()
            {
                Content = "overwrite",
                Value = "false"
            }
        };
    }
}