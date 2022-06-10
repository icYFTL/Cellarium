using System.Reflection;
using System.Text;
using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands;
using Utils = Utils.Utils;

[NoAuthNeeded]
public class HelpCommand : BaseCommand
{
    public sealed override string? Description { get; init; }
    public sealed override List<BaseAlias> Aliases { get; init; }
    public sealed override List<BaseParameter>? Parameters { get; init; }
    public override void Run(params BaseParameter [] arguments)
    {
        base.Run();
        
        const string commandsNameSpace = "Cellarium.Commands";

        var commands = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, commandsNameSpace, StringComparison.Ordinal) && !t.FullName!.Contains("<>"))
            .ToList();
        var sb = new StringBuilder($"Cellarium v{Utils.GetAssemblyVersion()}\n\nCommands:\n");

        foreach (var command in commands)
        {
            var instance = (BaseCommand)Activator.CreateInstance(command)!;
            sb.Append(String.Join(" or ", instance.Aliases.Select(x => x.AsArgument)));
            if (instance.Parameters?.Count > 0)
            {
                sb.Append(" [ ");
                foreach (var param in instance.Parameters ?? new List<BaseParameter>())
                {
                    sb.Append($"{param.AsArgument}={param.Value} ");
                }

                sb.Append("] ");
            }

            sb.Append($" -> {instance.Description}");
            sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }

    public HelpCommand()
    {
        Description = "Show help";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "h"
            },
            new ()
            {
                Type = AliasTypeEnum.Fully,
                Content = "help"
            }
        };
        Parameters = null;
    }
}