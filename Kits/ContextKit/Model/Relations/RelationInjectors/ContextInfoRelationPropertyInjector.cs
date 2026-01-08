using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

// context: relations, build
public class ContextInfoRelationPropertyInjector : IContextInfoRelationInjector
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;

    public ContextInfoRelationPropertyInjector(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
    }

    // context: relations, build

    public void Inject(Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> propertiesFullNames)
    {
        foreach (var fullName in propertiesFullNames)
        {
            if (lookupDictionary.TryGetValue(fullName, out var property))
            {
                InjectProperty(context, property);
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"Property not found {fullName}");
            }
        }
    }

    private void InjectProperty(ContextInfo context, ContextInfo property)
    {
        var addedToProperties = _contextInfoManager.AddToProperties(context, property);
        var message = addedToProperties
            ? $"[DONE] Adding property for [{context.FullName}] with [{property.FullName}]"
            : $"[SKIP] Adding property for [{context.FullName}] with [{property.FullName}]";
        var level = addedToProperties
            ? LogLevel.Trace
            : LogLevel.Err;
        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }
}
