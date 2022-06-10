using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Utils;

namespace Cellarium.Commands;

[NoAuthNeeded]
public class IsTokenSetCommand : BaseCommand
{
    public sealed override string? Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }

    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);

        if (Environment.GetEnvironmentVariable("token") == null)
            Logger.Warn("Token is not set");
        else
            Logger.Info("Token is set");
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
        Logger = new Logger().GetLogger<SetTokenCommand>();
    }
}