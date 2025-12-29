using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Services.Parsing;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoIndexerProvider : BaseContextInfoProvider, IContextInfoIndexerProvider
{
    private readonly IContextInfoIndexerFactory _mapperFactory;
    private readonly IAppOptionsStore _optionsStore;

    private readonly Dictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoIndexerProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoIndexerFactory mapperFactory)
        : base(parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
        _optionsStore = optionsStore;
    }

    public async Task<IKeyIndexBuilder<ContextInfo>> GetIndexerAsync(MapperKeyBase mapperType, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_mappers.TryGetValue(mapperType, out var mapper))
            {
                return mapper;
            }
        }

        // Маппер не найден, нужно его собрать.
        var contextsList = await GetParsedContextsAsync(cancellationToken).ConfigureAwait(false);

        var result = _mapperFactory.GetMapper(mapperType);
        result.Build(contextsList);

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