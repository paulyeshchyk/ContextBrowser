using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

// context: relations, build
public class ContextInfoRelationInvokedByInjector : IContextInfoRelationInjector
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;

    public ContextInfoRelationInvokedByInjector(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
    }

    // context: relations, build

    public void Inject(Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> invokedByFullNames)
    {
        foreach (var fullName in invokedByFullNames)
        {
            if (lookupDictionary.TryGetValue(fullName, out var invokedBy))
            {
                InjectInvokedBy(context, invokedBy);
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"InvokedBy not found {fullName}");
            }
        }
    }

    private void InjectInvokedBy(ContextInfo context, ContextInfo invokedBy)
    {
        var addedInvokedBy = _contextInfoManager.AddToInvokedBy(context, invokedBy);
        var message = addedInvokedBy
            ? $"[DONE] Adding invokedBy for [{context.FullName}] with [{invokedBy.FullName}]"
            : $"[SKIP] Adding invokedBy for [{context.FullName}] with [{invokedBy.FullName}]";
        var level = addedInvokedBy
            ? LogLevel.Trace
            : LogLevel.Err;
        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }
}
