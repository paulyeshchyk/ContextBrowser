using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using CustomServers;
using Microsoft.Extensions.Hosting;

namespace ContextBrowser.Services;

public class CustomEnvironmentHostedService : IHostedService
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly IServerStartSignal _startSignal;
    private readonly IHostApplicationLifetime _appLifetime;

    public CustomEnvironmentHostedService(IAppOptionsStore optionsStore, IServerStartSignal startSignal, IHostApplicationLifetime appLifetime)
    {
        _optionsStore = optionsStore;
        _startSignal = startSignal;
        _appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            // Ожидаем сигнала от MainService, не блокируя основной поток
            await _startSignal.WaitForSignalAsync();

            var filePaths = _optionsStore.GetOptions<ExportFilePaths>();
            CustomEnvironment.CopyResources(filePaths.OutputDirectory);
            CustomEnvironment.RunServers(filePaths.OutputDirectory);
        }, cancellationToken).ConfigureAwait(false);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Здесь можно добавить логику для остановки серверов
        return Task.CompletedTask;
    }
}