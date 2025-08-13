using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders;

public interface IContextDiagramBuilder
{
    GrouppedSortedTransitionList? Build(string domainName, List<ContextInfo> allContexts, IContextClassifier classifier);
}