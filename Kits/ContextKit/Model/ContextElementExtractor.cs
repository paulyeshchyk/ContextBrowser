using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model.Classifier;

namespace ContextKit.Model;

public class ContextElementExtractor : IContextElementExtractor
{
    private readonly IContextClassifier _wordRoleClassifier;
    private readonly IFakeDimensionClassifier _fakeDimensionClassifier;

    public ContextElementExtractor(IAppOptionsStore appOptionsStore)
    {
        _wordRoleClassifier = appOptionsStore.GetOptions<IContextClassifier>();
        _fakeDimensionClassifier = appOptionsStore.GetOptions<IFakeDimensionClassifier>();
    }

    public ContextElementGroups Extract(ContextInfo item)
    {
        var groups = new ContextElementGroups
        {
            Verbs = item.Contexts
                .Where(c => _wordRoleClassifier.IsVerb(c, _fakeDimensionClassifier))
                .Distinct()
                .ToList(),

            Nouns = item.Contexts
                .Where(c => _wordRoleClassifier.IsNoun(c, _fakeDimensionClassifier))
                .Distinct()
                .ToList()
        };
        return groups;
    }
}