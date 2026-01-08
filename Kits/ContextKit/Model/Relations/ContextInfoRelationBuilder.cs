using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

public interface IContextInfoRelationBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    void BuildRelation(ContextInfoSerializableModel contextInfoSerializableModel, TContext context, Dictionary<string, ContextInfo> lookupDictionary);
}

// context: relations, build

public class ContextInfoRelationBuilder : IContextInfoRelationBuilder<ContextInfo>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly IEnumerable<IContextInfoRelationInjector> _descendantInjectors;
    private readonly IEnumerable<IContextInfoRelationOwnerInjector> _ownerInjectors;

    public ContextInfoRelationBuilder(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager, IEnumerable<IContextInfoRelationInjector> descendantInjectors, IEnumerable<IContextInfoRelationOwnerInjector> ownerInjectors)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
        _descendantInjectors = descendantInjectors;
        _ownerInjectors = ownerInjectors;
    }

    // context: relations, build

    public void BuildRelation(ContextInfoSerializableModel contextInfoSerializableModel, ContextInfo context, Dictionary<string, ContextInfo> lookupDictionary)
    {
        // Восстанавливаем ClassOwner и MethodOwner.
        foreach (var injector in _ownerInjectors)
        {
            injector.Inject(context, contextInfoSerializableModel, lookupDictionary);
        }

        // Восстанавливаем References, InvokedBy и Properties.
        foreach (var injector in _descendantInjectors)
        {
            injector.Inject(lookupDictionary, context, contextInfoSerializableModel.ReferencesFullNames);
        }
    }
}
