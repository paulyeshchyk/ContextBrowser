using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using System.Collections.Concurrent;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly List<ITransitionBuilder> _transitionBuilders;
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly ContextTransitionDiagramBuilderCache _cache;

    public ContextTransitionDiagramBuilder(ContextTransitionDiagramBuilderOptions options, IEnumerable<ITransitionBuilder> transitionBuilders, OnWriteLog? onWriteLog = null)
    {
        _transitionBuilders = transitionBuilders.Where(b => b.Direction == options.Direction).ToList();
        _onWriteLog = onWriteLog;
        _cache = new ContextTransitionDiagramBuilderCache(onWriteLog);
    }

    public SortedGrouppedTransitionList? Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier)
    {
        var resultFromCache = _cache.GetFromCache(domainName);
        if(resultFromCache?.Any() ?? false)
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"returning from cache");
            return resultFromCache;
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"ContextTransitionDiagramBuilder.Build transitions for {domainName}", LogLevelNode.Start);

        var result = Fetch(domainName, allContexts, classifier);

        _cache.AddToCache(domainName, result);

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }

    private SortedGrouppedTransitionList? Fetch(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier)
    {
        var methods = FetchAllMethods(domainName, allContexts, classifier);

        if(!methods.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"ContextTransitionDiagramBuilder.Fetch  Не найдено методов для [{domainName}]");
            return null;
        }

        return BuildAllTransitions(domainName, allContexts, methods);
    }

    private static List<ContextInfo> FetchAllMethods(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier)
    {
        return allContexts
            .Where(ctx =>
                ctx.ElementType == ContextInfoElementType.method &&
                ctx.Domains.Contains(domainName) &&
                classifier.HasActionAndDomain(ctx))
            .ToList();
    }

    private SortedGrouppedTransitionList BuildAllTransitions(string domainName, List<ContextInfo> allContexts, List<ContextInfo> methods)
    {
        var allTransitions = new SortedGrouppedTransitionList();

        if(!_transitionBuilders.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Err, $"Build Domain [{domainName}] - no transition builders provided for domain");
            return allTransitions;
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Build Domain [{domainName}]", LogLevelNode.Start);

        int globalCounter = 0;
        foreach(var builder in _transitionBuilders)
        {
            globalCounter += BuildWithConcreteBuilder(builder, allContexts, methods, allTransitions, globalCounter);
        }

        if(!allTransitions.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"No transitions found for domain [{domainName}]");
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return allTransitions;
    }

    private int BuildWithConcreteBuilder(ITransitionBuilder builder, List<ContextInfo> allContexts, List<ContextInfo> methods, SortedGrouppedTransitionList allTransitions, int globalIndex)
    {
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Building Transitions [{builder.GetType().Name}]", LogLevelNode.Start);

        var transitions = builder.BuildTransitions(methods, allContexts);

        foreach(var t in transitions)
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Save Transition [{t.CallerClassName}.{t.CallerMethod} -> {t.CalleeClassName}.{t.CalleeMethod}]");
            allTransitions.Add(globalIndex++, t);
        }

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return globalIndex;
    }
}

internal class ContextTransitionDiagramBuilderCache
{
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly ConcurrentDictionary<string, SortedGrouppedTransitionList> _cache = new();

    public ContextTransitionDiagramBuilderCache(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    public SortedGrouppedTransitionList? GetFromCache(string cacheKey)
    {
        // Попытка получить данные из кэша.
        if(_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Возвращаем кэшированный результат для [{cacheKey}]");
            return cachedResult;
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"В кеше не найдено элементов для [{cacheKey}]");

        return null;
    }

    public void AddToCache(string cacheKey, SortedGrouppedTransitionList? result)
    {
        if(result == null || !result.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Результат не передан для кеширования для [{cacheKey}]");
            return;
        }

        _cache.TryAdd(cacheKey, result);
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Кэшируем результат для [{cacheKey}]");
    }

    public void ClearCache()
    {
        _cache.Clear();
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, "Кэш билдера был очищен.");
    }
}