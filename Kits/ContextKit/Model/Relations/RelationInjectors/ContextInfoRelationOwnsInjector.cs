using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

public class ContextInfoRelationOwnsInjector : IContextInfoRelationInjector
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;

    public ContextInfoRelationOwnsInjector(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
    }

    // context: relations, build

    public void Inject(Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> ownsFullNames)
    {
        foreach (var fullName in ownsFullNames)
        {
            if (lookupDictionary.TryGetValue(fullName, out var property))
            {
                InjectOwns(context, property);
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"Property not found {fullName}");
            }
        }
    }

    private void InjectOwns(ContextInfo context, ContextInfo owns)
    {
        var addedToOwns = _contextInfoManager.AddToOwns(context, owns);
        var message = addedToOwns
            ? $"[DONE] Adding property for [{context.FullName}] with [{owns.FullName}]"
            : $"[SKIP] Adding property for [{context.FullName}] with [{owns.FullName}]";
        var level = addedToOwns
            ? LogLevel.Trace
            : LogLevel.Err;
        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }
}