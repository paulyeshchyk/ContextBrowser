using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

// context: relations, build

public class ContextInfoRelationReferenceInjector : IContextInfoRelationInjector
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;

    public ContextInfoRelationReferenceInjector(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
    }

    // context: relations, build

    public void Inject(Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> referencesFullNames)
    {
        foreach (var fullName in referencesFullNames)
        {
            if (lookupDictionary.TryGetValue(fullName, out var reference))
            {
                InjectReference(context, reference);
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"Reference not found {fullName}");
            }
        }
    }

    internal void InjectReference(ContextInfo context, ContextInfo reference)
    {
        var addedReference = _contextInfoManager.AddToReferences(context, reference);
        var message = (addedReference)
            ? $"[DONE] Adding reference for [{context.FullName}] with [{reference.FullName}]"
            : $"[SKIP] Adding reference for [{context.FullName}] with [{reference.FullName}]";
        var level = addedReference
            ? LogLevel.Trace
            : LogLevel.Err;

        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }
}
