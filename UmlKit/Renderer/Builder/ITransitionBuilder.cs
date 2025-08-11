using ContextKit.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Renderer.Model;

namespace UmlKit.Renderer.Builder;

public interface ITransitionBuilder
{
    DiagramDirection Direction { get; }

    GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts);
}
