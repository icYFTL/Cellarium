using Cellarium.Commands.Parameters;
using Cellarium.Handlers;
using Cellarium.Utils;

var logger = new Logger().GetLogger("Main");

if (File.Exists(".token"))
{
    var token = File.ReadAllText(".token").Trim();

    if (String.IsNullOrEmpty(token))
        File.Delete(Path.Join(AppDomain.CurrentDomain.BaseDirectory, ".token"));
    else
        Environment.SetEnvironmentVariable("token", token);
}

var parsedArgs = ArgsHandler.Parse();

if (parsedArgs.Item1 == null)
{
    Console.WriteLine("Type -h or --help for help.");
    Environment.Exit(0);
}

try
{
    parsedArgs.Item1.Run(parsedArgs.Item2 ?? Array.Empty<BaseParameter>());
}
catch (Exception ex)
{
    logger.Fatal(ex.Message);
}