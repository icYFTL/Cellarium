using Cellarium.Api;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;

public sealed class ClearExpiredCommand : BaseCommand
{
    public override void Run(params BaseParameter [] arguments)
    {
        base.Run(arguments);
        
        var externalPath = arguments.First(x => x.Content == "external_path").Value;

        if (!int.TryParse(arguments.First(x => x.Content == "delta").Value, out var delta))
            throw new ArgumentException("Bad delta passed");

        var deletePermanently = false;
        var _dp = arguments.FirstOrDefault(x => x.Content == "delete_permanently")?.Value;
        if (!String.IsNullOrEmpty(_dp))
            if (!Boolean.TryParse(_dp, out deletePermanently))
                throw new ArgumentException("Bad delete_permanently passed");

        var yandexCloudApi = new YandexCloudApi(new DiskHttpApi(Environment.GetEnvironmentVariable("token")));
        var handler = new IoHandler(yandexCloudApi, externalPath);
        handler.ClearExpired(delta, deletePermanently);
    }

    public ClearExpiredCommand()
    {
        Description = "Deletes old files";
        FullDescription = @"Deletes all files which are older provided delta
external_path - Path to clear
delta - Max delta between now and date when file was created (in days)
delete_permanently - Send files to trash or delete forever";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "ce"
            },
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "clear_expired"
            }
        };
        Parameters = new List<BaseParameter>
        {
            new ()
            {
                Content = "external_path",
                Value = "/",
                Optional = false
            },
            new ()
            {
                Content = "delta",
                Value = "30",
                Optional = false
            },
            new ()
            {
                Content = "delete_permanently",
                Value = "false"
            }
        };
    }
}