using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using UmlKit.Builders.Strategies;
using UmlKit.Builders.TransitionDirection;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders;

public interface IContextDiagramBuildersFactory
{
    IEnumerable<string> AvailableNames { get; }

    // Методы для получения билдера по ключу
    IContextDiagramBuilder BuilderForType(DiagramBuilderKeys key, DiagramBuilderOptions options, IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager, IAppOptionsStore optionsStore);

    // Метод для регистрации новых билдеров (хотя это лучше делать в DI)
    void RegisterBuilder(string name, Func<DiagramBuilderOptions, IAppLogger<AppLevel>, IContextInfoManager<ContextInfo>, IAppOptionsStore, IContextDiagramBuilder> factoryFunc);

    // Метод для управления жизненным циклом кэша
    void ClearCache();
}
