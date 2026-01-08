using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.Model.Factory;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

// context: relations, build
public interface IContextInfoRelationManager
{
    //context: relations, build
    Task<IEnumerable<ContextInfo>> ConvertToContextInfoAsync(List<ContextInfoSerializableModel> serializableModelList, CancellationToken cancellationToken);
}

// context: relations, build
public class ContextInfoRelationManager : IContextInfoRelationManager
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly IContextInfoRelationBuilder<ContextInfo> _contextInfoRelationBuilder;

    public ContextInfoRelationManager(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager, IContextInfoRelationBuilder<ContextInfo> contextInfoRelationBuilder)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
        _contextInfoRelationBuilder = contextInfoRelationBuilder;
    }

    // context: relations, build
    public Task<IEnumerable<ContextInfo>> ConvertToContextInfoAsync(List<ContextInfoSerializableModel> serializableModelList, CancellationToken cancellationToken)
    {
        var contexts = serializableModelList.Select(ContextInfoAdapter.Adapt).ToList();

        var lookupDictionary = contexts.ToDictionary(context => context.FullName);

        for (int i = 0; i < contexts.Count; i++)
        {
            var context = contexts[i];
            var contextInfoSerializableModel = serializableModelList[i];

            _contextInfoRelationBuilder.BuildRelation(
                contextInfoSerializableModel: contextInfoSerializableModel,
                context: context,
                lookupDictionary);
        }

        IEnumerable<ContextInfo> result = contexts;
        return Task.FromResult(result);
    }
}
