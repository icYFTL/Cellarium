using Cellarium.Commands.Aliases;
using Cellarium.Commands.Parameters;
using NLog;
using Logger = Cellarium.Utils.Logger;

namespace Cellarium.Commands.Base;

public class BaseCommand
{
    public virtual string Description { get; init; }
    public virtual List<BaseAlias> Aliases { get; init; }
    public virtual List<BaseParameter>? Parameters { get; init; }
    protected ILogger _logger { get; init; }
    public bool NeedToBeFirst { get; init; } = false;
    public virtual void Run(params BaseParameter [] arguments)
    {
        var _params = arguments.Select(x => x.AsArgument);
        
        foreach (var param in Parameters?.Where(x => !x.Optional) ?? new List<BaseParameter>())
        {
            if (!_params.Contains(param.AsArgument))
            {
                _logger.Fatal($"Missing parameter: {param.Content}");
                Environment.Exit(0);
            }    
        }
    }
    
}