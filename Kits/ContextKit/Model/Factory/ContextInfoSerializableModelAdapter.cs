using System.Collections.Generic;
using System.Linq;
using ContextBrowser;
using ContextBrowser.FileManager;
using ContextKit.Model;
using ContextKit.Model.Factory;

namespace ContextKit.Model.Factory;

public static class ContextInfoSerializableModelAdapter
{
    // context: roslyncache, convert
    public static List<ContextInfoSerializableModel> Adapt(List<ContextInfo> contextsList)
    {
        return contextsList.Select(c => Adapt(c)).ToList();
    }

    // context: roslyncache, convert
    public static ContextInfoSerializableModel Adapt(ContextInfo contextInfo)
    {
        return new ContextInfoSerializableModel
        (
                    elementType: contextInfo.ElementType,
                           name: contextInfo.Name,
                       fullName: contextInfo.FullName,
                      shortName: contextInfo.ShortName,
                       contexts: contextInfo.Contexts,
                      nameSpace: contextInfo.Namespace,
             classOwnerFullName: contextInfo.ClassOwner?.FullName,
            methodOwnerFullName: contextInfo.MethodOwner?.FullName,
                         action: contextInfo.Action,
                        domains: contextInfo.Domains,
            referencesFullNames: contextInfo.References.Select(r => r.FullName).ToHashSet(),
             invokedByFullNames: contextInfo.InvokedBy.Select(i => i.FullName).ToHashSet(),
            propertiesFullNames: contextInfo.Properties.Select(p => p.FullName).ToHashSet(),
                     dimensions: contextInfo.Dimensions,
                      spanStart: contextInfo.SpanStart,
                        spanEnd: contextInfo.SpanEnd,
                     identifier: contextInfo.Identifier,
                  ownsFullNames: contextInfo.Owns.Select(o => o.FullName).ToHashSet());
    }
}
