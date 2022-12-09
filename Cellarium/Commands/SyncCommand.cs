using Cellarium.Api;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;

public sealed class SyncCommand : BaseCommand
{

    public override void Run(params BaseParameter [] arguments)
    {
        base.Run(arguments);
        
        var internalPath = arguments.First(x => x.Content == "internal_path").Value;
        var externalPath = arguments.First(x => x.Content == "external_path").Value;
        var tag = arguments.FirstOrDefault(x => x.Content == "tag")?.Value ?? "";
        
        
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

        /*var compress = false;
        var _comp = arguments.FirstOrDefault(x => x.Content == "compress")?.Value;
        if (!String.IsNullOrEmpty(_comp))
            if (!bool.TryParse(_comp, out compress))
                throw new ArgumentException("Bad compress passed");*/
        
        var yandexCloudApi = new YandexCloudApi(new DiskHttpApi(Environment.GetEnvironmentVariable("token")), clear);
        var handler = new IoHandler(yandexCloudApi, externalPath);
        handler.TransferToCloud(internalPath, tag, overwrite, forceCreateExternalPath);
    }

    public SyncCommand()
    {
        Description = "Syncs specified directory with yandex drive";
        FullDescription = @"Send all files from specified directory to the yandex drive
internal_path - Internal path to sync
external_path - External path on yandex drive
tag - Tag of upload
force_create_external_path - Creates external path if not exists
clear - Clear transferred files on local machine
overwrite - Overwrite files on yandex drive if exists";
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
            new()
            {
                Content = "tag",
                Value = "<empty>",
                Optional = true
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
            },
            // new ()
            // {
            //     Content = "compress",
            //     Value = "false"
            // }
        };
    }
}