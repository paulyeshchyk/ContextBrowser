using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoMappingProvider : BaseContextInfoProvider, IContextInfoMapperProvider
{
    private readonly IContextInfoMapperFactory _mapperFactory;

    private readonly Dictionary<MapperKeyBase, IContextKeyMap<ContextInfo>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoMappingProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoMapperFactory mapperFactory)
        : base(optionsStore, parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    public async Task<IContextKeyMap<ContextInfo>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken)
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
        var newMapper = _mapperFactory.GetMapper(mapperType);
        newMapper.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier);

        lock (_lock)
        {
            if (!_mappers.ContainsKey(mapperType))
            {
                _mappers[mapperType] = newMapper;
            }
        }

        return _mappers[mapperType];
    }
}
