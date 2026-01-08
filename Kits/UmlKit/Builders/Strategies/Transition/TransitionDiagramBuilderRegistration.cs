using System;
using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.Strategies;

public class TransitionDiagramBuilderRegistration : IDiagramBuilderRegistration<TransitionDiagramBuilderKey>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IEnumerable<ITransitionBuilder> _transitionBuilders;

    public Type KeyType => typeof(TransitionDiagramBuilderKey);

    public string Key => new TransitionDiagramBuilderKey().ToKeyString(); // Явно создает ключ

    public TransitionDiagramBuilderRegistration(
        IAppLogger<AppLevel> logger,
        IContextInfoManager<ContextInfo> contextInfoManager,
        IAppOptionsStore optionsStore,
        IEnumerable<ITransitionBuilder> transitionBuilders)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
        _optionsStore = optionsStore;
        _transitionBuilders = transitionBuilders;
    }

    public IContextDiagramBuilder CreateBuilder(DiagramBuilderOptions options)
    {
        return new TransitionDiagramBuilder(options, _transitionBuilders, _logger, _optionsStore);
    }
}