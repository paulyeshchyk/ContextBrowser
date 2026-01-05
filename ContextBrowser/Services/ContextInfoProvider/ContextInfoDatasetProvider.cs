using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Services.Parsing;
using ContextKit.Model;

namespace ContextBrowser.Services.ContextInfoProvider;

// context: ContextInfo, read
public class ContextInfoDatasetProvider<TTensor> : BaseContextInfoProvider, IContextInfoDatasetProvider<TTensor>
    where TTensor : notnull
{
    private readonly IContextInfoDatasetBuilder<TTensor> _datasetBuilder;

    private Task<IContextInfoDataset<ContextInfo, TTensor>>? _datasetTask;

    private readonly object _lock = new object();

    public ContextInfoDatasetProvider(IParsingOrchestrator parsingOrchestrant, IContextInfoDatasetBuilder<TTensor> datasetBuilder)
        : base(parsingOrchestrant)
    {
        _datasetBuilder = datasetBuilder;
    }

    // context: build, compilationFlow
    public async Task<IContextInfoDataset<ContextInfo, TTensor>> GetDatasetAsync(CancellationToken cancellationToken)
    {
        // Первая проверка без лока, для быстрого выхода
        if (_datasetTask != null)
        {
            // Возвращаем результат существующей задачи
            return await _datasetTask.ConfigureAwait(false);
        }

        // Если задача не инициализирована, используем lock
        lock (_lock)
        {
            // Если задача все еще null, создаем и запускаем задачу инициализации.
            // Task.Run гарантирует, что работа GetParsedContextsAsync и Build
            // выполняется вне текущего потока и НЕ блокирует его.
            _datasetTask ??= BuildDatasetTaskAsync(cancellationToken);
        }

        // Ожидаем завершения созданной или существующей задачи.
        return await _datasetTask.ConfigureAwait(false);
    }

    // context: build, compilationFlow
    internal async Task<IContextInfoDataset<ContextInfo, TTensor>> BuildDatasetTaskAsync(CancellationToken cancellationToken)
    {
        var contextsList = await GetParsedContextsAsync(cancellationToken).ConfigureAwait(false);
        return _datasetBuilder.Build(contextsList, cancellationToken);
    }
}
