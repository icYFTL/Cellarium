using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Utils;

namespace Cellarium.Commands;

public class RemoveTokenCommand : BaseCommand
{
    public sealed override string? Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }

    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);

        if (File.Exists(Path.Join(AppDomain.CurrentDomain.BaseDirectory, ".token")))
        {
            File.Delete(Path.Join(AppDomain.CurrentDomain.BaseDirectory, ".token"));
            Logger.Info("Token was removed");    
        }
        else
        {
            Logger.Info("Token does not exist");    
        }
        
    }

    public RemoveTokenCommand()
    {
        Description = "Remove yandex drive token";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "rmt"
            },
            new()
            {
                Type = AliasTypeEnum.Fully,
                Content = "remove_token"
            }
        };
        Parameters = null;
        Logger = new Logger().GetLogger<RemoveTokenCommand>();
    }
}