using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.Extensions.Hosting;

namespace ContextBrowser.Services;

public class CustomEnvironmentHostedService : IHostedService
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly IServerStartSignal _startSignal;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IAppLogger<AppLevel> _logger;

    public CustomEnvironmentHostedService(IAppOptionsStore optionsStore, IServerStartSignal startSignal, IHostApplicationLifetime appLifetime, IAppLogger<AppLevel> logger)
    {
        _optionsStore = optionsStore;
        _startSignal = startSignal;
        _appLifetime = appLifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {

        _ = Task.Run(async () =>
        {
            // Ожидаем сигнала от MainService, не блокируя основной поток
            await _startSignal.WaitForSignalAsync();

            var appOptions = _optionsStore.Options();
            CustomEnvironment.CopyResources(appOptions.Export.Paths.OutputDirectory);
            CustomEnvironment.RunServers(appOptions.Export.Paths.OutputDirectory, _logger.WriteLog);
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Здесь можно добавить логику для остановки серверов
        return Task.CompletedTask;
    }
}