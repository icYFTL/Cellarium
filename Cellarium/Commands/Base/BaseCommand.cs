using Cellarium.Commands.Aliases;
using Cellarium.Commands.Parameters;
using NLog;

namespace Cellarium.Commands.Base;

public class BaseCommand
{
    public virtual string? Description { get; init; }
    public virtual List<BaseAlias> Aliases { get; init; } = null!;
    public virtual List<BaseParameter>? Parameters { get; init; }
    protected ILogger Logger { get; init; } = null!;
    public bool NeedToBeFirst { get; init; } = false;
    public virtual void Run(params BaseParameter [] arguments)
    {
        var args = arguments.Select(x => x.AsArgument).ToList();
        
        foreach (var param in Parameters?.Where(x => !x.Optional) ?? new List<BaseParameter>())
        {
            if (!args.Contains(param.AsArgument))
            {
                Logger.Fatal($"Missing parameter: {param.Content}");
                Environment.Exit(-1);
            }    
        }
    }
    
}