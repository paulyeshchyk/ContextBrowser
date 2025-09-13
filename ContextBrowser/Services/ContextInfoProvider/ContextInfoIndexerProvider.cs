using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoIndexerProvider : BaseContextInfoProvider, IContextInfoIndexerProvider
{
    private readonly IContextInfoIndexerFactory _mapperFactory;

    private readonly Dictionary<MapperKeyBase, IContextKeyIndexer<ContextInfo>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoIndexerProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoIndexerFactory mapperFactory)
        : base(optionsStore, parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    public async Task<IContextKeyIndexer<ContextInfo>> GetIndexerAsync(MapperKeyBase mapperType, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_mappers.TryGetValue(mapperType, out var mapper))
            {
                return mapper;
            }
        }

        // Маппер не найден, нужно его собрать.
        var contextsList = await GetParsedContextsAsync(cancellationToken);
        var appOptions = _optionsStore.Options();

        var result = _mapperFactory.GetMapper(mapperType);
        result.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier);

        lock (_lock)
        {
            if (!_mappers.ContainsKey(mapperType))
            {
                _mappers[mapperType] = result;
            }
        }

        return _mappers[mapperType];
    }
}