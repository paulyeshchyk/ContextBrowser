using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Infrastucture;
using TensorKit.Model;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoMappingProviderDomainPerAction : BaseContextInfoProvider, IContextInfoMapperProvider
{
    private readonly IContextInfoMapperFactory _mapperFactory;

    private readonly Dictionary<MapperKeyBase, DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoMappingProviderDomainPerAction(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoMapperFactory mapperFactory)
        : base(optionsStore, parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    public async Task<DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken)
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
