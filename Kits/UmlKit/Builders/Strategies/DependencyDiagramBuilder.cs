using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders.Strategies;

public class DependencyDiagramBuilder : IContextDiagramBuilder
{
    public GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts, IContextClassifier classifier)
    {
        throw new NotImplementedException();
    }
}