using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ContextBrowserKit.Log;
using ContextBrowserKit.Options;
using LoggerKit;
using UmlKit.Builders.Strategies;
using UmlKit.Builders.TransitionDirection;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders;

public static class ContextDiagramBuildersFactory
{
    public static IEnumerable<string> AvailableNames => _builderFactories.Keys;

    private static readonly ConcurrentDictionary<string, IContextDiagramBuilder> _builderInstances = new(StringComparer.OrdinalIgnoreCase);

    public static IContextDiagramBuilder BuilderForType(DiagramBuilderKeys key, DiagramBuilderOptions options, IAppLogger<AppLevel> logger) => GetOrCreate(key, options, logger);

    public static IContextDiagramBuilder TransitionBuilder(DiagramBuilderOptions options, IAppLogger<AppLevel> logger) => GetOrCreate(DiagramBuilderKeys.Transition, options, logger);

    public static IContextDiagramBuilder DependenciesBuilder(DiagramBuilderOptions options, IAppLogger<AppLevel> logger) => GetOrCreate(DiagramBuilderKeys.Dependencies, options, logger);

    public static IContextDiagramBuilder MethodsOnlyBuilder(DiagramBuilderOptions options, IAppLogger<AppLevel> logger) => GetOrCreate(DiagramBuilderKeys.MethodFlow, options, logger);

    private static readonly Dictionary<string, Func<DiagramBuilderOptions, IAppLogger<AppLevel>, IContextDiagramBuilder>> _builderFactories = new(StringComparer.OrdinalIgnoreCase);

    private static List<ITransitionBuilder> DefaultDirectionBuilders(IAppLogger<AppLevel> logger) => new() {
            new OutgoingTransitionBuilder(logger),
            new IncomingTransitionBuilder(logger),
            new BiDirectionalTransitionBuilder(logger),
        };

    static ContextDiagramBuildersFactory()
    {
        RegisterBuilder(DiagramBuilderKeys.Transition, (options, logger) => new ContextTransitionDiagramBuilder(options, DefaultDirectionBuilders(logger), logger));
        RegisterBuilder(DiagramBuilderKeys.MethodFlow, (options, logger) => new MethodFlowDiagramBuilder());
        RegisterBuilder(DiagramBuilderKeys.Dependencies, (options, logger) => new DependencyDiagramBuilder());
    }

    public static void RegisterBuilder(string name, Func<DiagramBuilderOptions, IAppLogger<AppLevel>, IContextDiagramBuilder> factoryFunc)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Builder name cannot be null or whitespace.", nameof(name));
        if (factoryFunc == null)
            throw new ArgumentNullException(nameof(factoryFunc));

        if (_builderFactories.ContainsKey(name))
        {
            Console.WriteLine($"Warning: Builder '{name}' is already registered and will be overwritten.");
            _builderFactories[name] = factoryFunc;
        }
        else
        {
            _builderFactories.Add(name, factoryFunc);
        }
    }

    private static void RegisterBuilder(DiagramBuilderKeys key, Func<DiagramBuilderOptions, IAppLogger<AppLevel>, IContextDiagramBuilder> factoryFunc)
    {
        RegisterBuilder(key.ToKeyString(), factoryFunc);
    }

    private static IContextDiagramBuilder GetOrCreate(string name, DiagramBuilderOptions options, IAppLogger<AppLevel> logger)
    {
        return _builderInstances.GetOrAdd(name, key =>
        {
            if (!_builderFactories.TryGetValue(key, out var factoryFunc))
                throw new ArgumentException($"Builder not found for key '{key}'. Ensure it is registered.");

            return factoryFunc(options, logger);
        });
    }

    private static IContextDiagramBuilder GetOrCreate(DiagramBuilderKeys key, DiagramBuilderOptions options, IAppLogger<AppLevel> logger)
    {
        return GetOrCreate(key.ToKeyString(), options, logger);
    }

    public static void ClearCache()
    {
        _builderInstances.Clear();
    }
}