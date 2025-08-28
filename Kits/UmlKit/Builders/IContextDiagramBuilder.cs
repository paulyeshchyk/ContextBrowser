using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders;

// context: build, transition
public interface IContextDiagramBuilder
{
    GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts, IContextClassifier classifier);
}

public enum FetchType
{
    FetchAction,
    FetchDomain
}