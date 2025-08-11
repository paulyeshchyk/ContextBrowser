using ContextBrowserKit.Log;
using System.Collections.Concurrent;
using UmlKit.Infrastructure.Options;
using UmlKit.Renderer.Builder;
using UmlKit.Renderer.Builder.TransitionDirectionBuilder;

namespace UmlKit.Builders;

public static class ContextDiagramFactory
{
    public static IEnumerable<string> AvailableNames => _builderFactories.Keys;

    private static readonly ConcurrentDictionary<string, IContextDiagramBuilder> _builderInstances = new(StringComparer.OrdinalIgnoreCase);

    public static IContextDiagramBuilder Custom(DiagramBuilderKeys key, ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(key, options, onWriteLog);

    public static IContextDiagramBuilder Transition(ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(DiagramBuilderKeys.Transition, options, onWriteLog);

    public static IContextDiagramBuilder Dependencies(ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(DiagramBuilderKeys.Dependencies, options, onWriteLog);

    public static IContextDiagramBuilder MethodsOnly(ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => GetOrCreate(DiagramBuilderKeys.MethodFlow, options, onWriteLog);

    private static readonly Dictionary<string, Func<ContextTransitionDiagramBuilderOptions, OnWriteLog?, IContextDiagramBuilder>> _builderFactories = new(StringComparer.OrdinalIgnoreCase);

    private static List<ITransitionBuilder> DefaultDirectionBuilders(OnWriteLog? onWriteLog) => new() {
            new OutgoingTransitionBuilder(onWriteLog),
            new IncomingTransitionBuilder(onWriteLog),
            new BiDirectionalTransitionBuilder(onWriteLog),
        };

    // Статический конструктор для инициализации фабрики
    static ContextDiagramFactory()
    {
        RegisterBuilder(DiagramBuilderKeys.Transition,(options, onWriteLog) => new ContextTransitionDiagramBuilder(options, DefaultDirectionBuilders(onWriteLog), onWriteLog));
        RegisterBuilder(DiagramBuilderKeys.MethodFlow,(options, onWriteLog) => new MethodFlowDiagramBuilder());
        RegisterBuilder(DiagramBuilderKeys.Dependencies,(options, onWriteLog) => new DependencyDiagramBuilder());
    }

    public static void RegisterBuilder(string name, Func<ContextTransitionDiagramBuilderOptions, OnWriteLog?, IContextDiagramBuilder> factoryFunc)
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

    private static void RegisterBuilder(DiagramBuilderKeys key, Func<ContextTransitionDiagramBuilderOptions, OnWriteLog?, IContextDiagramBuilder> factoryFunc)
    {
        RegisterBuilder(key.ToKeyString(), factoryFunc);
    }

    private static IContextDiagramBuilder GetOrCreate(string name, ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null)
    {
        return _builderInstances.GetOrAdd(name, key =>
        {
            if(!_builderFactories.TryGetValue(key, out var factoryFunc))
                throw new ArgumentException($"Builder not found for key '{key}'. Ensure it is registered.");

            return factoryFunc(options, onWriteLog);
        });
    }

    private static IContextDiagramBuilder GetOrCreate(DiagramBuilderKeys key, ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null)
    {
        return GetOrCreate(key.ToKeyString(), options, onWriteLog);
    }

    public static void ClearCache()
    {
        _builderInstances.Clear();
    }
}