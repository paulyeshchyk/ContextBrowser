using ContextBrowser.DiagramFactory.Builders;
using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

namespace ContextBrowser.DiagramFactory;

public static class ContextDiagramFactory
{
    /// <summary>
    /// Возвращает все зарегистрированные имена билдеров.
    /// </summary>
    public static IEnumerable<string> AvailableNames => _builderFactories.Keys;

    public static IContextDiagramBuilder Transition(ContextTransitionDiagramBuilderOptions? options = null) => Get(DiagramBuilderKeys.Transition, options);

    public static IContextDiagramBuilder Dependencies(ContextTransitionDiagramBuilderOptions? options = null) => Get(DiagramBuilderKeys.Dependencies, options);

    public static IContextDiagramBuilder MethodsOnly(ContextTransitionDiagramBuilderOptions? options = null) => Get(DiagramBuilderKeys.MethodFlow, options);

    private static readonly Dictionary<string, Func<ContextTransitionDiagramBuilderOptions?, IContextDiagramBuilder>> _builderFactories = new(StringComparer.OrdinalIgnoreCase);

    private static List<ITransitionBuilder> DefaultDirectionBuilders => new() {
            new OutgoingTransitionBuilder(),
            new IncomingTransitionBuilder(),
            new BiDirectionalTransitionBuilder(),
        };


    // Статический конструктор для инициализации фабрики
    static ContextDiagramFactory()
    {
        // Используем Enum.ToKeyString() для регистрации
        RegisterBuilder(DiagramBuilderKeys.Transition,(options) => new ContextTransitionDiagramBuilder(options, DefaultDirectionBuilders));
        RegisterBuilder(DiagramBuilderKeys.MethodFlow,(options) => new MethodFlowDiagramBuilder());
        RegisterBuilder(DiagramBuilderKeys.Dependencies,(options) => new DependencyDiagramBuilder());
    }

    public static void RegisterBuilder(string name, Func<ContextTransitionDiagramBuilderOptions?, IContextDiagramBuilder> factoryFunc)
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

    private static void RegisterBuilder(DiagramBuilderKeys key, Func<ContextTransitionDiagramBuilderOptions?, IContextDiagramBuilder> factoryFunc)
    {
        RegisterBuilder(key.ToKeyString(), factoryFunc);
    }

    private static IContextDiagramBuilder Get(string name, ContextTransitionDiagramBuilderOptions? options = null)
    {
        if(!_builderFactories.TryGetValue(name, out var factoryFunc))
            throw new ArgumentException($"Builder not found for key '{name}'. Ensure it is registered.");

        return factoryFunc(options);
    }

    private static IContextDiagramBuilder Get(DiagramBuilderKeys key, ContextTransitionDiagramBuilderOptions? options = null)
    {
        return Get(key.ToKeyString(), options);
    }
}

public enum DiagramBuilderKeys
{
    Transition, // "context-transition"
    MethodFlow, // "method-flow" (прежнее "MethodsOnly")
    Dependencies // "dependencies"
}

public static class DiagramBuilderKeyExtensions
{
    public static string ToKeyString(this DiagramBuilderKeys key)
    {
        return key.ToString().ToLowerInvariant();
    }
}
