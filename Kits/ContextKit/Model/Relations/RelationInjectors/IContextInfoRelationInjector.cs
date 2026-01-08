using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

public interface IContextInfoRelationInjector
{
    void Inject(Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> referencesFullNames);
}
