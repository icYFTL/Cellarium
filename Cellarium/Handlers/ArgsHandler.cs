using System.Reflection;
using Cellarium.Commands.Base;
using Cellarium.Commands.Parameters;

namespace Cellarium.Handlers;
public static class ArgsHandler
{
    public static Tuple<BaseCommand?, BaseParameter[]?> Parse()
    {
        const string nameSpace = "Cellarium.Commands";
        var args = Environment.GetCommandLineArgs().Skip(1).ToList();
        var commands = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && !t.FullName!.Contains("<>"))
            .ToList();

        var arg = args.Take(1).FirstOrDefault();
        if (arg == null)
            return new Tuple<BaseCommand?, BaseParameter[]?>(null, null);

        args.RemoveAt(0);

        var command = commands.Select(x => (BaseCommand) Activator.CreateInstance(x, null)!).FirstOrDefault(x =>
            x.Aliases.Any(y => y.Content == arg.ToLower().Trim().Replace("--", "").Replace("-", "")))
            ?.GetType();

        if (command == null)
        {
            Console.WriteLine($"Not a valid arg: {arg}");
            Environment.Exit(-1);
        }

        var parameters = new List<BaseParameter>();
        
        foreach (var param in args)
        {
            var sParam = param.Split("=");
            if (sParam.Length != 2)
            {
                Console.WriteLine($"Not a valid parameter: {param}");
                Environment.Exit(-1);
            }

            parameters.Add(new BaseParameter
            {
                Content = sParam[0].ToLower().Trim().Replace("--", "").Replace("-", ""),
                Value = sParam[1]
            });
        }

        var result = (BaseCommand) Activator.CreateInstance(command)!;

        return new Tuple<BaseCommand?, BaseParameter[]?>(result, parameters.ToArray());
    }
    
}