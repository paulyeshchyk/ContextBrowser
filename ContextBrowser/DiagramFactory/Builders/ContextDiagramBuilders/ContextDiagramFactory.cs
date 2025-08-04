using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using LoggerKit;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

using ContextBrowser.Infrastructure;
using System.Collections.Concurrent;

public static class ContextDiagramFactory
{
    public static IEnumerable<string> AvailableNames => _builderFactories.Keys;

    private static readonly ConcurrentDictionary<string, IContextDiagramBuilder> _builderInstances = new(StringComparer.OrdinalIgnoreCase);

    public static IContextDiagramBuilder Custom(DiagramBuilderKeys key, AppOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(key, options, onWriteLog);

    public static IContextDiagramBuilder Transition(AppOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(DiagramBuilderKeys.Transition, options, onWriteLog);

    public static IContextDiagramBuilder Dependencies(AppOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(DiagramBuilderKeys.Dependencies, options, onWriteLog);

    public static IContextDiagramBuilder MethodsOnly(AppOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(DiagramBuilderKeys.MethodFlow, options, onWriteLog);

    private static readonly Dictionary<string, Func<AppOptions, OnWriteLog?, IContextDiagramBuilder>> _builderFactories = new(StringComparer.OrdinalIgnoreCase);

    private static List<ITransitionBuilder> DefaultDirectionBuilders(AppOptions options, OnWriteLog? onWriteLog) => new() {
            new OutgoingTransitionBuilder(options.roslynCodeparserOptions,onWriteLog),
            new IncomingTransitionBuilder(options.roslynCodeparserOptions,onWriteLog),
            new BiDirectionalTransitionBuilder(options.roslynCodeparserOptions,onWriteLog),
        };

    // Статический конструктор для инициализации фабрики
    static ContextDiagramFactory()
    {
        RegisterBuilder(DiagramBuilderKeys.Transition,(options, onWriteLog) => new ContextTransitionDiagramBuilder(options.contextTransitionDiagramBuilderOptions, DefaultDirectionBuilders(options, onWriteLog), onWriteLog));
        RegisterBuilder(DiagramBuilderKeys.MethodFlow,(options, onWriteLog) => new MethodFlowDiagramBuilder());
        RegisterBuilder(DiagramBuilderKeys.Dependencies,(options, onWriteLog) => new DependencyDiagramBuilder());
    }

    public static void RegisterBuilder(string name, Func<AppOptions, OnWriteLog?, IContextDiagramBuilder> factoryFunc)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Builder name cannot be null or whitespace.", nameof(name));
        if(factoryFunc == null)
            throw new ArgumentNullException(nameof(factoryFunc));

        if(_builderFactories.ContainsKey(name))
        {
            Console.WriteLine($"Warning: Builder '{name}' is already registered and will be overwritten.");
            _builderFactories[name] = factoryFunc;
        }
        else
        {
            _builderFactories.Add(name, factoryFunc);
        }
    }

    private static void RegisterBuilder(DiagramBuilderKeys key, Func<AppOptions, OnWriteLog?, IContextDiagramBuilder> factoryFunc)
    {
        RegisterBuilder(key.ToKeyString(), factoryFunc);
    }

    private static IContextDiagramBuilder GetOrCreate(string name, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        return _builderInstances.GetOrAdd(name, key =>
        {
            if(!_builderFactories.TryGetValue(key, out var factoryFunc))
                throw new ArgumentException($"Builder not found for key '{key}'. Ensure it is registered.");

            return factoryFunc(options, onWriteLog);
        });
    }

    private static IContextDiagramBuilder GetOrCreate(DiagramBuilderKeys key, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        return GetOrCreate(key.ToKeyString(), options, onWriteLog);
    }

    public static void ClearCache()
    {
        _builderInstances.Clear();
    }
}