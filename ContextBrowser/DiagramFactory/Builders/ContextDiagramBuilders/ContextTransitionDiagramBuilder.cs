using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextBrowser.DiagramFactory.Model;
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
    private readonly ConcurrentDictionary<string, SortedList<int, UmlTransitionDto>> _cache = new();

    public ContextTransitionDiagramBuilder(ContextTransitionDiagramBuilderOptions options, IEnumerable<ITransitionBuilder> transitionBuilders, OnWriteLog? onWriteLog = null)
    {
        _transitionBuilders = transitionBuilders.Where(b => b.Direction == options.Direction).ToList();
        _onWriteLog = onWriteLog;
    }

    public SortedList<int, UmlTransitionDto>? Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier)
    {
        var cacheKey = domainName;

        var resultFromCache = GetFromCache(domainName, cacheKey);
        if (resultFromCache?.Any() ?? false)
        {
            return resultFromCache;
        }

        var methods = allContexts
            .Where(ctx =>
                ctx.ElementType == ContextInfoElementType.method &&
                ctx.Domains.Contains(domainName) &&
                classifier.HasActionAndDomain(ctx))
            .ToList();

        if (!methods.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"Не найдено методов для [{domainName}]");
            return null;
        }

        var result = BuildAllTransitions(domainName, allContexts, methods);

        AddToCache(domainName, cacheKey, result);

        return result;
    }

    private SortedList<int, UmlTransitionDto>? GetFromCache(string domainName, string cacheKey)
    {
        // Попытка получить данные из кэша.
        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"Возвращаем кэшированный результат для [{domainName}]");
            return cachedResult;
        }
        _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"В кеше не найдено элементов для [{domainName}]");

        return null;
    }

    private void AddToCache(string domainName, string cacheKey, SortedList<int, UmlTransitionDto>? result)
    {
        // Если результат получен, добавляем его в кэш.
        if (result != null)
        {
            _cache.TryAdd(cacheKey, result);
            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"Кэшируем результат для [{domainName}]");
        }
    }

    private SortedList<int, UmlTransitionDto> BuildAllTransitions(string domainName, List<ContextInfo> allContexts, List<ContextInfo> methods)
    {
        var allTransitions = new SortedList<int, UmlTransitionDto>();

        _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"Build Domain [{domainName}]", LogLevelNode.Start);
        if (!_transitionBuilders.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Err, $"Build Domain [{domainName}] - no transition builders provided for domain");
            return allTransitions;
        }

        int globalCounter = 0;
        foreach (var builder in _transitionBuilders)
        {
            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"Building Transitions [{builder.GetType().Name}]", LogLevelNode.Start);

            var transitions = builder.BuildTransitions(methods, allContexts);

            foreach (var t in transitions)
            {
                _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"Save Transition [{t.CallerClassName}.{t.CallerMethod} -> {t.CalleeClassName}.{t.CalleeMethod}]");
                allTransitions.Add(globalCounter++, t);
            }

            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        }

        if (!allTransitions.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"No transitions found for domain [{domainName}]");
        }

        _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return allTransitions;
    }

    /// <summary>
    /// Очищает кэш для всех ранее построенных диаграмм.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
        _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, "Кэш билдера был очищен.");
    }
}