using Cellarium.Api;
using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Utils;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;

[NoAuthNeeded]
public class SetTokenCommand : BaseCommand
{
    public sealed override string Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }

    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);

        var token = arguments.First(x => x.Content == "token").Value;
        if (String.IsNullOrEmpty(token))
        {
            _logger.Fatal("Invalid token");
            Environment.Exit(-1);
        }

        try
        {
            var result = Task.Run(() => new YandexCloudApi(new DiskHttpApi(token)).GetFileListAsync("/")).Result;
        }
        catch
        {
            _logger.Fatal("Invalid token");
            Environment.Exit(-1);
        }

        File.WriteAllText(Path.Join(AppDomain.CurrentDomain.BaseDirectory, ".token"), token);
        _logger.Info("Token was set");
    }

    public SetTokenCommand()
    {
        Description = "Set yandex drive token";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "st"
            },
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "set_token"
            }
        };
        Parameters = new List<BaseParameter>
        {
            new ()
            {
                Content = "token",
                Value = "XXX",
                Optional = false
            },
        };
        _logger = new Logger().GetLogger<SetTokenCommand>();
    }
}