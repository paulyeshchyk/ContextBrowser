using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.Strategies;

// context: builder, transition
public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly List<ITransitionBuilder> _transitionBuilders;
    private readonly OnWriteLog? _onWriteLog;
    private readonly ContextTransitionDiagramBuilderCache _cache;
    private readonly DiagramBuilderOptions _options;

    public ContextTransitionDiagramBuilder(DiagramBuilderOptions options, IEnumerable<ITransitionBuilder> transitionBuilders, OnWriteLog? onWriteLog = null)
    {
        _options = options;
        _transitionBuilders = transitionBuilders.Where(b => b.Direction == _options.DiagramDirection).ToList();
        _onWriteLog = onWriteLog;
        _cache = new ContextTransitionDiagramBuilderCache(onWriteLog);
    }

    // context: builder, transition
    public GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts, IDomainPerActionContextClassifier classifier)
    {
        if (!allContexts.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Err, $"[CACHE]: No contexts provided for {metaItem}");
            return null;
        }

        var resultFromCache = _cache.GetFromCache(metaItem);
        if (resultFromCache?.HasTransitions() ?? false)
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: returning data for [{metaItem}]");
            return resultFromCache;
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Cntx, $"Fetch transitions for {fetchType} [{metaItem}]", LogLevelNode.Start);

        var result = FetchAndBuildTransitions(metaItem, fetchType, allContexts, classifier);

        _cache.AddToCache(metaItem, result);

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return result;
    }

    /// <summary>
    /// Унифицированный метод для выборки и построения переходов.
    /// </summary>
    // context: builder, transition
    internal GrouppedSortedTransitionList? FetchAndBuildTransitions(string name, FetchType fetchType, List<ContextInfo> allContexts, IDomainPerActionContextClassifier classifier)
    {
        var methods = FetchAllMethods(name, fetchType, allContexts, classifier);
        if (!methods.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Err, $"[MISS] Fetch Не найдено методов для {fetchType} [{name}]");
            return null;
        }
        return BuildAllTransitions(name, allContexts, methods);
    }

    // context: builder, transition
    internal static List<ContextInfo> FetchAllMethods(string name, FetchType fetchType, List<ContextInfo> allContexts, IDomainPerActionContextClassifier classifier)
    {
        Func<ContextInfo, string, bool> filterFunc = fetchType switch
        {
            FetchType.FetchDomain => (ctx, domain) => classifier.IsDomainApplicable(ctx, domain),
            FetchType.FetchAction => (ctx, action) => classifier.IsActionApplicable(ctx, action),
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

        if (!_transitionBuilders.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Err, $"Build Domain [{metaItem}] - no transition builders provided for domain");
            return allTransitions;
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Build Domain [{metaItem}]", LogLevelNode.Start);

        foreach (var builder in _transitionBuilders)
        {
            var transitions = builder.BuildTransitions(methods, allContexts);

            allTransitions.Merge(transitions);
        }

        if (!allTransitions.HasTransitions())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"No transitions found for domain [{metaItem}]");
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return allTransitions;
    }
}

// context: share, transitioncache
internal class ContextTransitionDiagramBuilderCache
{
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly ConcurrentDictionary<string, GrouppedSortedTransitionList> _cache = new();

    public ContextTransitionDiagramBuilderCache(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    // context: read, transitioncache
    public bool IsEmpty()
    {
        return !_cache.Any();
    }

    // context: read, transitioncache
    public GrouppedSortedTransitionList? GetFromCache(string cacheKey)
    {
        // Попытка получить данные из кэша.
        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: return result for [{cacheKey}]");
            return cachedResult;
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: result is empty for [{cacheKey}]");

        return null;
    }

    // context: create, transitioncache
    public void AddToCache(string cacheKey, GrouppedSortedTransitionList? result)
    {
        if (result == null || !result.HasTransitions())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"[CACHE] Nothing to cache for [{cacheKey}]");
            return;
        }

        _cache.TryAdd(cacheKey, result);
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: Adding [{cacheKey}]");
    }

    // context: delete, transitioncache
    public void ClearCache()
    {
        _cache.Clear();
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, "[CACHE]: Wiped");
    }
}