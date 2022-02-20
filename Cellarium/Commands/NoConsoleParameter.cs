using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands;

[AsParameter]
[NoAuthNeeded]
public sealed class NoConsoleParameter : BaseCommand
{
    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);
        
        Environment.SetEnvironmentVariable("no_console", "1");
    }
    
    public NoConsoleParameter()
    {
        Description = "Disable console output";
        NeedToBeFirst = true;
        Aliases = new List<BaseAlias>
        {
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "no_console"
            }
        };
        Parameters = null;
    }
}