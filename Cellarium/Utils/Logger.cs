using System.Text;
using NLog;
using NLog.Targets;

namespace Cellarium.Utils;

public class Logger
{
    private LogFactory _factory;
    public Logger()
    {
        var logger = LogManager.Setup().LoadConfigurationFromFile($"NLog.Release.config");
        var verbose = Environment.GetEnvironmentVariable("verbose") == "1";
        var noConsole = Environment.GetEnvironmentVariable("no_console") == "1";
        
        if (!noConsole)
        {
            var consoleTarget = new ConsoleTarget("logconsole");
            consoleTarget.DetectConsoleAvailable = true;
            consoleTarget.AutoFlush = true;
            consoleTarget.Encoding = Encoding.UTF8;
            consoleTarget.Layout = "${message}";
            logger.LogFactory.Configuration.AddTarget(consoleTarget);
            logger.LogFactory.Configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);
        }

        _factory = logger.LogFactory;
        _factory.ReconfigExistingLoggers();
    }

    public ILogger GetLogger<T>()
    {
        return _factory.GetLogger(nameof(T)); // :(
    }
    
    public ILogger GetLogger(string name)
    {
        return _factory.GetLogger(name);
    }
}