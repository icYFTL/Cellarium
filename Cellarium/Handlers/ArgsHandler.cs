using System.Reflection;
using Cellarium.Attributes;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;
using Cellarium.Utils;

namespace Cellarium.Handlers;
public static class ArgsHandler
{
    public static Tuple<BaseCommand?, BaseParameter[]?> Parse()
    {
        var logger = new Logger().GetLogger(nameof(ArgsHandler));
        
        const string nameSpace = "Cellarium.Commands";
        var args = Environment.GetCommandLineArgs().Skip(1).ToList();
        var commands = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && !t.FullName!.Contains("<>"))
            .Select(x => (BaseCommand) Activator.CreateInstance(x, null)!)
            .ToList();

        // var arg = args.Take(1).FirstOrDefault();
        // if (arg == null)
        //     return new Tuple<BaseCommand?, BaseParameter[]?>(null, null);

        if (!args.Any())
        {
            logger.Info("Use -h or --help");
            Environment.Exit(0);
        }
            
        
        
        // var parsedCommands = commands.Select(x => (BaseCommand) Activator.CreateInstance(x, null)!).Where(x =>
        //     x.Aliases.Any(y => y.Content == arg.ToLower().Trim().Replace("--", "").Replace("-", ""))).ToList();


        var parsedCommands = new List<BaseCommand>();

        foreach (var arg in args)
        {
            var cmd = commands.FirstOrDefault(x =>
                x.Aliases.Any(y => y.Content == arg.ToLower().Trim().Replace("--", "").Replace("-", "")));

            // if (cmd.NeedToBeFirst)
            // {
            //     cmd.Run();
            //     continue;
            // }
            
            if (cmd != null)
                parsedCommands.Add(cmd);
        }
        
        

        foreach (var ntb in parsedCommands.Where(x => x.NeedToBeFirst))
        {
            ntb.Run();
            foreach (var alias in ntb.Aliases.Select(x => x.AsArgument))
            {
                try // TODO: Refactor
                {
                    args.Remove(alias);
                }
                catch
                {
                    
                }
            }
            
        }
        parsedCommands = parsedCommands.Where(x => !x.NeedToBeFirst).ToList();

        logger = new Logger().GetLogger(nameof(ArgsHandler));
        
        if (parsedCommands.Count > 1)
        {
            logger.Fatal("Too many commands in a row.");
            Environment.Exit(-1);
        }
        
        else if (!parsedCommands.Any())
        {
            logger.Info("Use -h or --help");
            Environment.Exit(0);
        }

        var command = parsedCommands[0];
        args = args.Where(x => !command.Aliases.Select(y => y.AsArgument).Contains(x)).ToList();

        if (!Attribute.IsDefined(command.GetType(), typeof(NoAuthNeeded)) &&
            String.IsNullOrEmpty(Environment.GetEnvironmentVariable("token")))
        {
            logger.Fatal("Auth needed. Use --set_token to set the token.");
            Environment.Exit(-1);
        }

        var parameters = new List<BaseParameter>();
        
        foreach (var param in args)
        {
            var sParam = param.Split("=");
            if (sParam.Length != 2)
            {
                logger.Fatal($"Not a valid parameter: {param}");
                Environment.Exit(-1);
            }

            parameters.Add(new BaseParameter
            {
                Content = sParam[0].ToLower().Trim().Replace("--", "").Replace("-", ""),
                Value = sParam[1]
            });
        }

        return new Tuple<BaseCommand?, BaseParameter[]?>(command, parameters.ToArray());
    }
    
}