using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders;

public interface ITransitionBuilder
{
    DiagramDirection Direction { get; }

    GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts);
}
