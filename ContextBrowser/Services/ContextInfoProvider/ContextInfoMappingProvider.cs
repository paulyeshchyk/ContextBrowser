using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoMappingProvider : BaseContextInfoProvider, IContextInfoMapperProvider
{
    private readonly IContextInfoMapperFactory _mapperFactory;

    // Словарь для кэширования мапперов по типу.
    private readonly Dictionary<MapperType, IContextKeyMap<ContextInfo>> _mappers = new();

    // Объект для синхронизации доступа.
    private readonly object _lock = new();

    public ContextInfoMappingProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoMapperFactory mapperFactory)
        : base(optionsStore, parsingOrchestrant)
    {
        _mapperFactory = mapperFactory;
    }

    // Метод для асинхронного получения уже сформированного маппера.
    public async Task<IContextKeyMap<ContextInfo>> GetMapperAsync(MapperType mapperType, CancellationToken cancellationToken)
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
        var newMapper = _mapperFactory.CreateMapper(mapperType);
        newMapper.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier);

        lock (_lock)
        {
            // Убеждаемся, что другой поток не добавил маппер, пока мы его собирали.
            if (!_mappers.ContainsKey(mapperType))
            {
                _mappers[mapperType] = newMapper;
            }
        }

        return _mappers[mapperType];
    }
}
