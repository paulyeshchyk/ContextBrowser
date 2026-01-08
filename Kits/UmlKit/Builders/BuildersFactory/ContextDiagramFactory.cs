using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using UmlKit.Builders.Strategies;
using UmlKit.Builders.TransitionDirection;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders;

public class ContextDiagramBuildersFactory : IContextDiagramBuildersFactory
{
    private readonly Dictionary<Type, IDiagramBuilderRegistration> _builderRegistrations;
    private readonly ConcurrentDictionary<string, IContextDiagramBuilder> _builderInstances = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<DiagramBuilderOptions, IAppLogger<AppLevel>, IContextInfoManager<ContextInfo>, IAppOptionsStore, IContextDiagramBuilder>> _builderFactories = new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> AvailableNames => _builderFactories.Keys;

    public IContextDiagramBuilder BuilderForType(DiagramBuilderKeys key, DiagramBuilderOptions options, IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager, IAppOptionsStore optionsStore) => GetOrCreate(key, options, logger, contextInfoManager, optionsStore);

    public ContextDiagramBuildersFactory(IEnumerable<IDiagramBuilderRegistration> registrations)
    {
        _builderRegistrations = registrations
            .GroupBy(reg => reg.KeyType)
            .ToDictionary(g => g.Key, g => g.First());

        // Заполняем _builderFactories для обратной совместимости с существующими методами GetOrCreate
        foreach (var reg in registrations)
        {
            // Используем Key (который теперь должен быть предоставлен регистрацией)
            // Использование ключа из регистрации обеспечивает, что мы используем правильную строку
            RegisterBuilder(
                reg.Key,
                (options, logger, contextInfoManager, optionsStore) => reg.CreateBuilder(options)
            );
        }
    }

    public IContextDiagramBuilder BuilderForType<TKey>(DiagramBuilderOptions options, IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager, IAppOptionsStore optionsStore)
            where TKey : IDiagramBuilderKey
    {
        if (_builderRegistrations.TryGetValue(typeof(TKey), out var registration))
        {
            // Здесь мы обходим кэш GetOrCreate, если он нам не нужен,
            // или модифицируем GetOrCreate, чтобы он использовал Type.

            // Если мы хотим использовать кэш GetOrCreate, нам нужно получить строковый ключ:
            var keyInstance = (TKey)Activator.CreateInstance(typeof(TKey))!;
            return GetOrCreate(keyInstance.ToKeyString(), options, logger, contextInfoManager, optionsStore);
        }
        throw new ArgumentException($"Builder not found for key type '{typeof(TKey).Name}'.");
    }

    public void RegisterBuilder(string name, Func<DiagramBuilderOptions, IAppLogger<AppLevel>, IContextInfoManager<ContextInfo>, IAppOptionsStore, IContextDiagramBuilder> factoryFunc)
    {
        ArgumentNullException.ThrowIfNull(factoryFunc);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Builder name cannot be null or whitespace.", nameof(name));

        var added = _builderFactories.TryAdd(name, factoryFunc);
        if (!added)
        {
            Console.WriteLine($"Warning: Builder '{name}' is already registered and will be overwritten.");
            _builderFactories[name] = factoryFunc;
        }
    }

    public IContextDiagramBuilder GetOrCreate(string name, DiagramBuilderOptions options, IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager, IAppOptionsStore optionsStore)
    {
        return _builderInstances.GetOrAdd(name, key =>
        {
            if (!_builderFactories.TryGetValue(key, out var factoryFunc))
                throw new ArgumentException($"Builder not found for key '{key}'. Ensure it is registered.");

            return factoryFunc(options, logger, contextInfoManager, optionsStore);
        });
    }

    public IContextDiagramBuilder GetOrCreate(DiagramBuilderKeys key, DiagramBuilderOptions options, IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager, IAppOptionsStore optionsStore)
    {
        return GetOrCreate(key.ToKeyString(), options, logger, contextInfoManager, optionsStore);
    }

    public void ClearCache()
    {
        _builderInstances.Clear();
    }
}