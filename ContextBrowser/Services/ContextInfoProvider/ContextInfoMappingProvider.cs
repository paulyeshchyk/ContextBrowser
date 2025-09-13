using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoMappingProvider : BaseContextInfoProvider, IContextInfoMapperProvider
{
    private readonly IContextInfoMapperFactory _mapperFactory;

    private readonly Dictionary<MapperKeyBase, IContextKeyMap<ContextInfo, IContextKey>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoMappingProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoMapperFactory mapperFactory)
        : base(optionsStore, parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    public async Task<IContextKeyMap<ContextInfo, IContextKey>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken)
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
