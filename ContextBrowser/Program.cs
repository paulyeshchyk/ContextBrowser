using System;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Infrastructure;
using ContextBrowser.Infrastructure.Options;
using ContextBrowserKit.Model;

namespace ContextBrowser;

// context: app, model
public static class Program
{
    public static async Task Main(string[] args)
    {
        var appOptions = await AppOptionsResolver.ResolveOptionsAsync(args, CancellationToken.None);
        if (appOptions == null)
        {
            Environment.Exit(1);
        }

        var runnerTask = appOptions.ExecutionMode switch
        {
            AppExecutionMode.Console => ConsoleRunner.Run(args, appOptions),
            AppExecutionMode.WebApp => WebAppRunner.Run(args, appOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(appOptions.ExecutionMode))
        };

        await runnerTask.ConfigureAwait(false);
    }
}
