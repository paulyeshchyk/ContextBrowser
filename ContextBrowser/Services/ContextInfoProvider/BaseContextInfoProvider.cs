﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser;
using ContextBrowser.Services;
using ContextBrowser.Services.ContextInfoProvider;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;

namespace ContextBrowser.Services.ContextInfoProvider;

// Абстрактный базовый класс для провайдеров, которым нужен список контекстов.
public abstract class BaseContextInfoProvider
{
    protected readonly IAppOptionsStore _optionsStore;
    protected readonly IParsingOrchestrator _parsingOrchestrant;

    // Поле для кэширования результата.
    private IEnumerable<ContextInfo>? _contextsList;

    // Объект для синхронизации доступа к полю.
    private readonly object _lock = new object();

    protected BaseContextInfoProvider(IAppOptionsStore optionsStore, IParsingOrchestrator parsingOrchestrant)
    {
        _optionsStore = optionsStore;
        _parsingOrchestrant = parsingOrchestrant;
    }

    protected async Task<IEnumerable<ContextInfo>> GetParsedContextsAsync(CancellationToken cancellationToken)
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

        return _contextsList!;
    }
}
