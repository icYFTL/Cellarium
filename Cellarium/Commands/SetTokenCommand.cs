using Cellarium.Api;
using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Utils;
using YandexDisk.Client.Http;

namespace Cellarium.Commands;

[NoAuthNeeded]
public sealed class SetTokenCommand : BaseCommand
{

    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);

        var token = arguments.First(x => x.Content == "token").Value;
        if (String.IsNullOrEmpty(token))
        {
            Logger.Fatal("Invalid token");
            Environment.Exit(-1);
        }

        try
        {
            var result = Task.Run(() => new YandexCloudApi(new DiskHttpApi(token)).GetFileListAsync("/")).Result;
        }
        catch
        {
            Logger.Fatal("Invalid token");
            Environment.Exit(-1);
        }

        File.WriteAllText(Path.Join(AppDomain.CurrentDomain.BaseDirectory, ".token"), token);
        Logger.Info("Token was set");
    }

    public SetTokenCommand()
    {
        Description = "Sets yandex drive token";
        FullDescription = @"Sets yandex drive token to .token file in cellarium base path
token - Token from yandex";
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
        Logger = new Logger().GetLogger<SetTokenCommand>();
    }
}