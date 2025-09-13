using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Factory;
using LoggerKit;

namespace ContextBrowser.FileManager;

// context: relations, build
public interface IContextInfoRelationManager
{
    //relations, build
    Task<IEnumerable<ContextInfo>> ConvertToContextInfoAsync(List<ContextInfoSerializableModel> serializableModelList, CancellationToken cancellationToken);
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
    public Task<IEnumerable<ContextInfo>> ConvertToContextInfoAsync(List<ContextInfoSerializableModel> serializableModelList, CancellationToken cancellationToken)
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

        IEnumerable<ContextInfo> result = contexts;
        return Task.FromResult(result);
    }
}
