using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands;
using Utils = Cellarium.Utils.Utils;

public class VersionCommand : BaseCommand
{
    public sealed override string Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }

    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);
        Console.WriteLine($"v{Utils.GetAssemblyVersion()}");
    }

    public VersionCommand()
    {
        Description = "Get cellarium version";
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