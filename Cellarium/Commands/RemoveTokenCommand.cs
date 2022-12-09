using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Utils;

namespace Cellarium.Commands;

public sealed class RemoveTokenCommand : BaseCommand
{
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
        Description = "Removes yandex drive token";
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