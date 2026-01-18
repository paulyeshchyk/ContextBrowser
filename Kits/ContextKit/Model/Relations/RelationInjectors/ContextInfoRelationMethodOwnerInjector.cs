using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

// context: relations, build

public class ContextInfoRelationMethodOwnerInjector : IContextInfoRelationOwnerInjector
{
    // context: relations, build
    public void Inject(ContextInfo context, ContextInfoSerializableModel contextInfoSerializableModel, Dictionary<string, ContextInfo> lookupDictionary)
    {
        if (contextInfoSerializableModel.MethodOwnerFullName != null && lookupDictionary.ContainsKey(contextInfoSerializableModel.MethodOwnerFullName))
        {
            context.MethodOwner = lookupDictionary[contextInfoSerializableModel.MethodOwnerFullName];
        }
    }
}
