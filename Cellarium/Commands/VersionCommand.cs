using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands;

[NoAuthNeeded]
public sealed class VersionCommand : BaseCommand
{
    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);
        Console.WriteLine($"{Utils.Utils.GetAssemblyVersion()}");
    }

    public VersionCommand()
    {
        Description = "Gets cellarium version";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "v"
            },
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "version"
            }
        };
    }
}