using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;

namespace ContextBrowser.FileManager;

// context: relations, build
public interface IContextInfoRelationManager
{
    //relations, build
    List<ContextInfo> ConvertToContextInfo(List<ContextInfoSerializableModel> serializableList);
}

// context: relations, build
public class ContextInfoRelationManager : IContextInfoRelationManager
{
    private readonly IAppLogger<AppLevel> _logger;

    public ContextInfoRelationManager(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: relations, build
    public List<ContextInfo> ConvertToContextInfo(List<ContextInfoSerializableModel> serializableModelList)
    {
        var contexts = serializableModelList.Select(model => ContextInfoAdapter.Adapt(model)).ToList();

        var lookupDictionary = contexts.ToDictionary(context => context.FullName);

        for (int i = 0; i < contexts.Count; i++)
        {
            var context = contexts[i];
            var contextInfoSerializableModel = serializableModelList[i];

            ContextInfoRelationBuilder.BuildRelation(
                contextInfoSerializableModel: contextInfoSerializableModel,
                context: context,
                _logger,
                lookupDictionary);
        }

        return contexts;
    }
}
