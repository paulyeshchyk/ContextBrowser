using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Infrastucture;
using TensorKit.Model;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoMappingProviderDomainPerAction<TKey> : BaseContextInfoProvider, IContextInfoMapperProvider<TKey>
    where TKey : notnull
{
    private readonly IContextInfoMapperFactory<TKey> _mapperFactory;

    private readonly Dictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, TKey>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoMappingProviderDomainPerAction(IParsingOrchestrator parsingOrchestrant, IContextInfoMapperFactory<TKey> mapperFactory) : base(parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    public async Task<IContextInfo2DMap<ContextInfo, TKey>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_mappers.TryGetValue(mapperType, out var mapper))
            {
                return mapper;
            }
        }

        var contextsList = await GetParsedContextsAsync(cancellationToken);

        var result = _mapperFactory.GetMapper(mapperType);

        await result.BuildAsync(contextsList, cancellationToken).ConfigureAwait(false);

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
