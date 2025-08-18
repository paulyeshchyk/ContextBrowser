using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders;

// context: build, transition
public interface IContextDiagramBuilder
{
    // context: build, transition
    GrouppedSortedTransitionList? Build(string domainName, List<ContextInfo> allContexts, IContextClassifier classifier);
}