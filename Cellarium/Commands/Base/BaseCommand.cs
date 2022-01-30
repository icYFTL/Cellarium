using Cellarium.Commands.Aliases;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands.Base;

public class BaseCommand : IBaseCommand
{
    public virtual string Description { get; init; }
    public virtual List<BaseAlias> Aliases { get; init; }
    public virtual List<BaseParameter>? Parameters { get; init; }

    public virtual void Run(params BaseParameter [] arguments)
    {
        var _params = arguments.Select(x => x.AsArgument);
        
        foreach (var param in Parameters?.Where(x => !x.Optional) ?? new List<BaseParameter>())
        {
            if (!_params.Contains(param.AsArgument))
            {
                Console.WriteLine($"Missing parameter: {param.Content}");
                Environment.Exit(0);
            }    
        }
    }
    
}