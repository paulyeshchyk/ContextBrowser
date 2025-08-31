using System.Text.Json;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextBrowser.FileManager;

// context: relations, build
public static class ContextInfoAdapter
{
    // context: relations, build
    public static ContextInfo Adapt(ContextInfoSerializableModel model)
    {
        return new ContextInfo(
            elementType: model.ElementType,
             identifier: model.Identifier,
                   name: model.Name,
               fullName: model.FullName,
              shortName: model.ShortName,
              nameSpace: model.Namespace,
              spanStart: model.SpanStart,
                spanEnd: model.SpanEnd,
               contexts: model.Contexts,
                 action: model.Action,
                domains: model.Domains,
             dimensions: model.Dimensions)
        { };
    }
}
