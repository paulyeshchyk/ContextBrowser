using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders;

// context: transition, build
public interface ITransitionBuilder
{
    DiagramDirection Direction { get; }

    // context: transition, build
    GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts);
}
