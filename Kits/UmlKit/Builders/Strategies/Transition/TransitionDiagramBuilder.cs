using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.Strategies;

// context: builder, transition
public class TransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly List<ITransitionBuilder> _transitionBuilders;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly TransitionDiagramBuilderCache _cache;
    private readonly ITensorClassifierDomainPerActionContext<ContextInfo> _contextClassifier;

    public TransitionDiagramBuilder(DiagramBuilderOptions options, IEnumerable<ITransitionBuilder> transitionBuilders, IAppLogger<AppLevel> logger, IAppOptionsStore optionsStore)
    {
        _transitionBuilders = transitionBuilders.Where(b => b.Direction == options.DiagramDirection).ToList();
        _logger = logger;
        _cache = new TransitionDiagramBuilderCache(_logger);
        _contextClassifier = optionsStore.GetOptions<ITensorClassifierDomainPerActionContext<ContextInfo>>();
    }

    // context: builder, transition
    public GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts)
    {
        if (allContexts.Count == 0)
        {
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Err, $"[CACHE]: No contexts provided for {metaItem}");
            return null;
        }

        var resultFromCache = _cache.GetFromCache(metaItem);
        if (resultFromCache?.HasTransitions() ?? false)
        {
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: returning data for [{metaItem}]");
            return resultFromCache;
        }

        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Cntx, $"Fetch transitions for {fetchType} [{metaItem}]", LogLevelNode.Start);

        var result = FetchAndBuildTransitions(metaItem, fetchType, allContexts);

        _cache.AddToCache(metaItem, result);

        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return result;
    }

    /// <summary>
    /// Унифицированный метод для выборки и построения переходов.
    /// </summary>
    // context: builder, transition
    internal GrouppedSortedTransitionList? FetchAndBuildTransitions(string name, FetchType fetchType, List<ContextInfo> allContexts)
    {
        var methods = FetchAllMethods(name, fetchType, allContexts, _contextClassifier);
        if (methods.Count == 0)
        {
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Err, $"[MISS] Fetch Не найдено методов для {fetchType} [{name}]");
            return null;
        }
        return BuildAllTransitions(name, allContexts, methods);
    }

    // context: builder, transition
    internal static List<ContextInfo> FetchAllMethods(string name, FetchType fetchType, List<ContextInfo> allContexts, ITensorClassifierDomainPerActionContext<ContextInfo> contextClassifier)
    {
        Func<ContextInfo, string, bool> filterFunc = fetchType switch
        {
            FetchType.FetchDomain => (ctx, domain) => contextClassifier.IsDimensionApplicable(ctx, domain, DomainPerActionDimensionType.Domain),
            FetchType.FetchAction => (ctx, action) => contextClassifier.IsDimensionApplicable(ctx, action, DomainPerActionDimensionType.Action),
            _ => throw new ArgumentOutOfRangeException(nameof(fetchType), $"Unsupported FetchType: {fetchType}")
        };

        var result = allContexts
            .Where(ctx => ctx.ElementType == ContextInfoElementType.method && filterFunc(ctx, name))
            .ToList();
        return result;
    }

    // context: builder, transition
    internal GrouppedSortedTransitionList BuildAllTransitions(string metaItem, List<ContextInfo> allContexts, List<ContextInfo> methods)
    {
        var allTransitions = new GrouppedSortedTransitionList();

        if (_transitionBuilders.Count == 0)
        {
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Err, $"Build Domain [{metaItem}] - no transition builders provided for domain");
            return allTransitions;
        }

        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, $"Build Domain [{metaItem}]", LogLevelNode.Start);

        foreach (var builder in _transitionBuilders)
        {
            var transitions = builder.BuildTransitions(methods, allContexts);

            allTransitions.Merge(transitions);
        }

        if (!allTransitions.HasTransitions())
        {
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, $"No transitions found for domain [{metaItem}]");
        }

        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return allTransitions;
    }
}

// context: share, transition, cache
internal class TransitionDiagramBuilderCache
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ConcurrentDictionary<string, GrouppedSortedTransitionList> _cache = new();

    public TransitionDiagramBuilderCache(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: read, transitioncache
    public GrouppedSortedTransitionList? GetFromCache(string cacheKey)
    {
        // Попытка получить данные из кэша.
        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: return result for [{cacheKey}]");
            return cachedResult;
        }
        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: result is empty for [{cacheKey}]");

        return null;
    }

    // context: create, transition, cache
    public void AddToCache(string cacheKey, GrouppedSortedTransitionList? result)
    {
        if (result == null || !result.HasTransitions())
        {
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, $"[CACHE] Nothing to cache for [{cacheKey}]");
            return;
        }

        _cache.TryAdd(cacheKey, result);
        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: Adding [{cacheKey}]");
    }
}
