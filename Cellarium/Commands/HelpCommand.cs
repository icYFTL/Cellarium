using System.Reflection;
using System.Text;
using Cellarium.Attributes;
using Cellarium.Commands.Aliases;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands;

[NoAuthNeeded]
public sealed class HelpCommand : BaseCommand
{
    public override void Run(params BaseParameter[] arguments)
    {
        base.Run(arguments);

        var comm = arguments.FirstOrDefault(x => x.Content == "command")?.Value;

        const string commandsNameSpace = "Cellarium.Commands";

        var commands = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, commandsNameSpace, StringComparison.Ordinal) &&
                        !t.FullName!.Contains("<>")).ToList();

        var sb = new StringBuilder($"Cellarium v{Utils.Utils.GetAssemblyVersion()}\n\n");
        if (!String.IsNullOrEmpty(comm))
        {
            sb.Append($"Command [{comm}]:\n");
            var found = false;
            foreach (var command in commands.OrderByDescending(x => x.FullName))
            {
                var instance = (BaseCommand)Activator.CreateInstance(command)!;
                if (instance.Aliases.Select(x => x.Content).Contains(comm.ToLower()))
                {
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

                    sb.Append($" -> {instance.Description}\n");
                    sb.Append($"{instance.FullDescription ?? "No full description provided for this command"}");
                    found = true;
                    break;
                }
            }

            if (!found)
                sb.Append($"Command [{comm}] not found");
        }
        else
        {
            sb.Append("Commands:\n");
            foreach (var command in commands)
            {
                var instance = (BaseCommand)Activator.CreateInstance(command)!;
                sb.Append(String.Join(" or ", instance.Aliases.Select(x => x.AsArgument)));
                if (instance.Parameters?.Count > 0)
                {
                    sb.Append(" [ ");
                    foreach (var param in instance.Parameters ?? new List<BaseParameter>())
                    {
                        sb.Append($"{param.AsParameter}");
                        if (param.Value is not null)
                        {
                            sb.Append($"={param.Value} ");
                        }
                    }

                    sb.Append("] ");
                }

                sb.Append($" -> {instance.Description}");
                sb.AppendLine();
            }
        }


        Console.Write(sb.ToString());
    }

    public HelpCommand()
    {
        Description = "Shows help";
        FullDescription = @"Just help command, nothing interesting :D
command - Any command from -h";
        Aliases = new List<BaseAlias>
        {
            new()
            {
                Type = AliasTypeEnum.Abbreviation,
                Content = "h"
            },
            new()
            {
                Type = AliasTypeEnum.Fully,
                Content = "help"
            }
        };
        Parameters = new()
        {
            new()
            {
                Content = "command",
                Value = "",
                Optional = true
            }
        };
    }
}