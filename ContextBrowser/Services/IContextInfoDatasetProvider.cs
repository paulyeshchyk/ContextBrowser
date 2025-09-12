using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;

namespace ContextBrowser.Services;

public class ContextInfoDatasetProvider : IContextInfoDatasetProvider
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly IParsingOrchestrator _parsingOrchestrant;
    private readonly IContextInfoDatasetBuilder _datasetBuilder;
    private readonly IContextInfoMapperFactory _mapperFactory;

    private IEnumerable<ContextInfo>? _contextsList;
    private IContextInfoDataset<ContextInfo>? _dataset;
    private IContextKeyMap<ContextInfo>? _mapper;

    // Объект для синхронизации доступа к полям.
    private readonly object _lock = new object();

    public ContextInfoDatasetProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoDatasetBuilder datasetBuilder,
        IContextInfoMapperFactory mapperFactory)
    {
        _optionsStore = optionsStore;
        _parsingOrchestrant = parsingOrchestrant;
        _datasetBuilder = datasetBuilder;
        _mapperFactory = mapperFactory;
    }

    // Метод для асинхронного получения уже сформированного датасета.
    public async Task<IContextInfoDataset<ContextInfo>> GetDatasetAsync(CancellationToken cancellationToken)
    {
        if (_dataset == null)
        {
            await BuildDatasetAsync(cancellationToken);
        }
        return _dataset!;
    }

    // Метод для асинхронного получения уже сформированного маппера.
    public async Task<IContextKeyMap<ContextInfo>> GetMapperAsync(CancellationToken cancellationToken)
    {
        if (_mapper == null)
        {
            await BuildMapperAsync(cancellationToken);
        }
        return _mapper!;
    }

    private async Task<IEnumerable<ContextInfo>> GetParsedContextsAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            // Если данные уже спарсены, возвращаем их.
            if (_contextsList != null)
            {
                return _contextsList;
            }
        }

        var appOptions = _optionsStore.Options();
        var contextsList = await _parsingOrchestrant.GetParsedContextsAsync(appOptions, cancellationToken);

        lock (_lock)
        {
            _contextsList = contextsList;
        }

        return _contextsList;
    }

    // для однократной сборки датасета.
    private async Task BuildDatasetAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_dataset != null)
            {
                return;
            }
        }

        var contextsList = await GetParsedContextsAsync(cancellationToken);
        var appOptions = _optionsStore.Options();
        var newDataset = _datasetBuilder.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier);

        lock (_lock)
        {
            _dataset = newDataset;
        }
    }

    // для однократной сборки маппера.
    private async Task BuildMapperAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_mapper != null)
            {
                return;
            }
        }

        var contextsList = await GetParsedContextsAsync(cancellationToken);
        var appOptions = _optionsStore.Options();
        var newMapper = _mapperFactory.CreateMapper(MapperType.DomainPerAction);
        newMapper.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier);

        lock (_lock)
        {
            _mapper = newMapper;
        }
    }
}
