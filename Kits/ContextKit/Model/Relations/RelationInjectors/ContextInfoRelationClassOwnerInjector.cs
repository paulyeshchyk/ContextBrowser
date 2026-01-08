using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

// context: relations, build
public class ContextInfoRelationClassOwnerInjector : IContextInfoRelationOwnerInjector
{
    // context: relations, build
    public void Inject(ContextInfo context, ContextInfoSerializableModel contextInfoSerializableModel, Dictionary<string, ContextInfo> lookupDictionary)
    {
        if (contextInfoSerializableModel.ClassOwnerFullName != null && lookupDictionary.ContainsKey(contextInfoSerializableModel.ClassOwnerFullName))
        {
            context.ClassOwner = lookupDictionary[contextInfoSerializableModel.ClassOwnerFullName];
        }
    }
}
