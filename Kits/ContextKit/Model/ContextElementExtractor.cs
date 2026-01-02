using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model.Classifier;

namespace ContextKit.Model;

public class ContextElementExtractor<TContext> : IContextElementExtractor<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextClassifier<TContext> _wordRoleClassifier;
    private readonly IFakeDimensionClassifier _fakeDimensionClassifier;

    public ContextElementExtractor(IAppOptionsStore appOptionsStore)
    {
        _wordRoleClassifier = appOptionsStore.GetOptions<IContextClassifier<TContext>>();
        _fakeDimensionClassifier = appOptionsStore.GetOptions<IFakeDimensionClassifier>();
    }

    public ContextElementGroups Extract(TContext item)
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