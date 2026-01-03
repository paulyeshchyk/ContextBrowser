using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Services.Parsing;
using ContextKit.Model;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Services.ContextInfoProvider;

// context: ContextInfo, read
public class ContextInfoIndexerProvider : BaseContextInfoProvider, IContextInfoIndexerProvider
{
    private readonly IContextInfoIndexerFactory _mapperFactory;

    private readonly Dictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoIndexerProvider(
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoIndexerFactory mapperFactory)
        : base(parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    // context: ContextInfo, read
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
            _mappers.TryAdd(mapperType, result);
        }

        return result;
    }
}