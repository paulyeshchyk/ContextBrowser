using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextBrowser.DiagramFactory.Model;
using LoggerKit;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public static class ContextDiagramFactory
{
    /// <summary>
    /// Возвращает все зарегистрированные имена билдеров.
    /// </summary>
    public static IEnumerable<string> AvailableNames => _builderFactories.Keys;

    public static IContextDiagramBuilder Custom(DiagramBuilderKeys key, ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => Get(key, options, onWriteLog);

    public static IContextDiagramBuilder Transition(ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => Get(DiagramBuilderKeys.Transition, options, onWriteLog);

    public static IContextDiagramBuilder Dependencies(ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => Get(DiagramBuilderKeys.Dependencies, options, onWriteLog);

    public static IContextDiagramBuilder MethodsOnly(ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null) => Get(DiagramBuilderKeys.MethodFlow, options, onWriteLog);

    private static readonly Dictionary<string, Func<ContextTransitionDiagramBuilderOptions, OnWriteLog?, IContextDiagramBuilder>> _builderFactories = new(StringComparer.OrdinalIgnoreCase);

    private static List<ITransitionBuilder> DefaultDirectionBuilders => new() {
            new OutgoingTransitionBuilder(),
            new IncomingTransitionBuilder(),
            new BiDirectionalTransitionBuilder(),
        };

    // Статический конструктор для инициализации фабрики
    static ContextDiagramFactory()
    {
        // Используем Enum.ToKeyString() для регистрации
        RegisterBuilder(DiagramBuilderKeys.Transition,(options, onWriteLog) => new ContextTransitionDiagramBuilder(options, DefaultDirectionBuilders, onWriteLog));
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

    private static IContextDiagramBuilder Get(string name, ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null)
    {
        if(!_builderFactories.TryGetValue(name, out var factoryFunc))
            throw new ArgumentException($"Builder not found for key '{name}'. Ensure it is registered.");

        return factoryFunc(options, onWriteLog);
    }

    private static IContextDiagramBuilder Get(DiagramBuilderKeys key, ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog = null)
    {
        return Get(key.ToKeyString(), options, onWriteLog);
    }
}