using Cellarium.Api;
using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using YandexDisk.Client.Http;
using Cellarium.Utils;

namespace Cellarium.Commands;

[NoAuthNeeded]
public class IsTokenSetCommand : BaseCommand
{
    public sealed override string Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }

    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);

        if (Environment.GetEnvironmentVariable("token") == null)
            _logger.Warn("Token is not set");
        else
            _logger.Info("Token is set");
    }

    public IsTokenSetCommand()
    {
        Description = "Check if token exists";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Fully,
                Content = "check_token"
            }
        };
        Parameters = null;
        _logger = new Logger().GetLogger<SetTokenCommand>();
    }
}