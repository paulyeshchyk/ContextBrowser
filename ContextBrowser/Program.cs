using System;
using System.Threading.Tasks;
using CommandlineKit;
using ContextBrowser.Infrastructure;
using LoggerKit.Model;

namespace ContextBrowser;

// context: app, model
public static class Program
{
    // context: app, execute
    public static async Task Main(string[] args)
    {

        var parser = new CommandlineArgumentsParserService();
        var options = parser.Parse<AppOptions>(args);

        if (options == null)
        {
            // Не удалось распарсить аргументы или была запрошена справка
            return;
        }

        switch (options.ExecutionMode)
        {
            case AppExecutionMode.Console:
                await ConsoleRunner.Run(args, options).ConfigureAwait(false);
                break;
            case AppExecutionMode.WebApp:
                await WebAppRunner.Run(args, options).ConfigureAwait(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}