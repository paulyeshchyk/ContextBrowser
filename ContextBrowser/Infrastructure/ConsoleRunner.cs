using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Services;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContextBrowser.Infrastructure;

// context: app, execute
internal static class ConsoleRunner
{
    // context: app, execute, compilationFlow
    public static async Task Run(string[] args, AppOptions options)
    {

        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        var hab = Host.CreateApplicationBuilder(args);

        HostConfigurator.ConfigureServices(hab.Services);
        hab.Services.AddHostedService<CustomEnvironmentHostedService>();

        hab.Services.AddSingleton<IMainService, MainService>();
        hab.Services.AddSingleton<IServerStartSignal, ServerStartSignal>();

        var tokenSource = new CancellationTokenSource();

        var host = hab.Build();

        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        Console.CancelKeyPress += (_, e) =>
        {
            Console.WriteLine("Прервано по требованию");
            e.Cancel = true;
            lifetime.StopApplication();
        };


        var optionsStore = host.Services.GetRequiredService<IAppOptionsStore>();
        optionsStore.SetOptions(options);

        var logger = host.Services.GetRequiredService<IAppLogger<AppLevel>>();
        logger.Configure(options.LogConfiguration);

        var mainService = host.Services.GetRequiredService<IMainService>();

        await host.StartAsync(tokenSource.Token).ConfigureAwait(false);

        try
        {
            await mainService.RunAsync(lifetime.ApplicationStopping);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            lifetime.StopApplication();
            await host.WaitForShutdownAsync(tokenSource.Token).ConfigureAwait(false);
            tokenSource.Dispose();
        }
    }

}
