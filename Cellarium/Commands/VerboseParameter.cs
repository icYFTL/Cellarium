using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands;

[AsParameter]
[NoAuthNeeded]
public sealed class VerboseParameter : BaseCommand
{
    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);
        
        Environment.SetEnvironmentVariable("verbose", "1");
    }
    
    public VerboseParameter()
    {
        Enabled = false;
        Description = "Verbose output";
        NeedToBeFirst = true;
        Aliases = new List<BaseAlias>
        {
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "verbose"
            }
        };
        Parameters = null;
    }
}