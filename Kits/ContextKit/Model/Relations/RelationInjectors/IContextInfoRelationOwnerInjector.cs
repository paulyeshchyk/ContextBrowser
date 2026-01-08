using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

public interface IContextInfoRelationOwnerInjector
{
    void Inject(ContextInfo context, ContextInfoSerializableModel contextInfoSerializableModel, Dictionary<string, ContextInfo> lookupDictionary);
}
