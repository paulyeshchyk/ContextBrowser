using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using System.Collections.Concurrent;
using UmlKit.Infrastructure.Options;
using UmlKit.Renderer.Builder;
using UmlKit.Renderer.Model;

namespace UmlKit.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly List<ITransitionBuilder> _transitionBuilders;
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly ContextTransitionDiagramBuilderCache _cache;
    private readonly ContextTransitionDiagramBuilderOptions _options;

    public ContextTransitionDiagramBuilder(ContextTransitionDiagramBuilderOptions options, IEnumerable<ITransitionBuilder> transitionBuilders, OnWriteLog? onWriteLog = null)
    {
        _options = options;
        _transitionBuilders = transitionBuilders.Where(b => b.Direction == _options.Direction).ToList();
        _onWriteLog = onWriteLog;
        _cache = new ContextTransitionDiagramBuilderCache(onWriteLog);
    }

    public GrouppedSortedTransitionList? Build(string domainName, List<ContextInfo> allContexts, IContextClassifier classifier)
    {
        var resultFromCache = _cache.GetFromCache(domainName);
        if(resultFromCache?.HasTransitions() ?? false)
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: returning data for [{domainName}]");
            return resultFromCache;
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Cntx, $"Fetch transitions for [{domainName}]", LogLevelNode.Start);

        var result = Fetch(domainName, allContexts, classifier);

        _cache.AddToCache(domainName, result);

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return result;
    }

    private GrouppedSortedTransitionList? Fetch(string domainName, List<ContextInfo> allContexts, IContextClassifier classifier)
    {
        var methods = FetchAllMethods(domainName, allContexts, classifier);

        if(!methods.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Warn, $"[MISS] Fetch Не найдено методов для [{domainName}]");
            return null;
        }

        return BuildAllTransitions(domainName, allContexts, methods);
    }

    private static List<ContextInfo> FetchAllMethods(string domainName, List<ContextInfo> allContexts, IContextClassifier classifier)
    {
        return allContexts
            .Where(ctx =>
                ctx.ElementType == ContextInfoElementType.method &&
                ctx.Domains.Contains(domainName) &&
                classifier.HasActionAndDomain(ctx))
            .ToList();
    }

    private GrouppedSortedTransitionList BuildAllTransitions(string domainName, List<ContextInfo> allContexts, List<ContextInfo> methods)
    {
        var allTransitions = new GrouppedSortedTransitionList();

        if(!_transitionBuilders.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Err, $"Build Domain [{domainName}] - no transition builders provided for domain");
            return allTransitions;
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Build Domain [{domainName}]", LogLevelNode.Start);

        foreach(var builder in _transitionBuilders)
        {
            BuildWithConcreteBuilder(builder, allContexts, methods, allTransitions);
        }

        if(!allTransitions.HasTransitions())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"No transitions found for domain [{domainName}]");
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return allTransitions;
    }

    private void BuildWithConcreteBuilder(ITransitionBuilder builder, List<ContextInfo> allContexts, List<ContextInfo> methods, GrouppedSortedTransitionList allTransitions)
    {
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Building Transitions [{builder.GetType().Name}]", LogLevelNode.Start);

        var transitions = builder.BuildTransitions(methods, allContexts);

        allTransitions.Merge(transitions);

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}

internal class ContextTransitionDiagramBuilderCache
{
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly ConcurrentDictionary<string, GrouppedSortedTransitionList> _cache = new();

    public ContextTransitionDiagramBuilderCache(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    public GrouppedSortedTransitionList? GetFromCache(string cacheKey)
    {
        // Попытка получить данные из кэша.
        if(_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: return result for [{cacheKey}]");
            return cachedResult;
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: result is empty for [{cacheKey}]");

        return null;
    }

    public void AddToCache(string cacheKey, GrouppedSortedTransitionList? result)
    {
        if(result == null || !result.HasTransitions())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Warn, $"[CACHE] Nothing to cache for [{cacheKey}]");
            return;
        }

        _cache.TryAdd(cacheKey, result);
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, $"[CACHE]: Adding [{cacheKey}]");
    }

    public void ClearCache()
    {
        _cache.Clear();
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Trace, "[CACHE]: Wiped");
    }
}