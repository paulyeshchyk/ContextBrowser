using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoMappingProvider<TTensor> : BaseContextInfoProvider, IContextInfoMapperProvider<TTensor>
    where TTensor : notnull
{
    private readonly IContextInfoMapperFactory<TTensor> _mapperFactory;

    private readonly Dictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, TTensor>> _mappers = new();

    private readonly object _lock = new();

    public ContextInfoMappingProvider(IParsingOrchestrator parsingOrchestrant, IContextInfoMapperFactory<TTensor> mapperFactory) : base(parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    public async Task<IContextInfo2DMap<ContextInfo, TTensor>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken)
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
