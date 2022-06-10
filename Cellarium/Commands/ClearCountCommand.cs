using Cellarium.Api;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;

public class ClearCountCommand : BaseCommand
{
    public sealed override string? Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }
    
    public override void Run(params BaseParameter [] arguments)
    {
        base.Run(arguments);
        
        var externalPath = arguments.First(x => x.Content == "external_path").Value;

        if (!int.TryParse(arguments.First(x => x.Content == "max_count").Value, out var maxCount))
        {
            throw new ArgumentException("Bad max_count passed");
        }

        var deletePermanently = false;
        var _dp = arguments.FirstOrDefault(x => x.Content == "delete_permanently")?.Value;
        if (!String.IsNullOrEmpty(_dp))
            if (!Boolean.TryParse(_dp, out deletePermanently))
                throw new ArgumentException("Bad delete_permanently passed");

        var yandexCloudApi = new YandexCloudApi(new DiskHttpApi(Environment.GetEnvironmentVariable("token")));
        var handler = new IoHandler(yandexCloudApi, externalPath);
        handler.ClearCount(maxCount, deletePermanently);
    }

    public ClearCountCommand()
    {
        Description = "Clear files by max count";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "cc"
            },
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "clear_count"
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
                Content = "max_count",
                Value = "10",
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