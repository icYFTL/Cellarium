using Cellarium.Commands.Aliases;
using Cellarium.Commands.Parameters;
using Cellarium.Utils;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Cellarium.Commands.Base;

public class BaseCommand
{
    protected readonly IConfiguration Configuration;
    public BaseCommand()
    {
        Configuration = Constants.Configuration;
    }
    public string? Description { get; init; }
    public string? FullDescription { get; init; }
    public List<BaseAlias> Aliases { get; init; } = null!;
    public List<BaseParameter>? Parameters { get; init; }
    protected ILogger Logger { get; init; } = null!;
    public bool NeedToBeFirst { get; init; } = false;
    public bool Enabled { get; init; } = true;
    public virtual void Run(params BaseParameter [] arguments)
    {
        if (!Enabled)
        {
            Console.WriteLine($"Command is disabled");
            Environment.Exit(-1);
        }
        var args = arguments.Select(x => x.AsArgument).ToList();
        
        foreach (var param in Parameters?.Where(x => !x.Optional) ?? new List<BaseParameter>())
        {
            if (!args.Contains(param.AsArgument))
            {
                Console.WriteLine($"Missing parameter: {param.Content}");
                Environment.Exit(-1);
            }    
        }
    }
    
}