using ContextKit.Model;
using UmlKit.Renderer.Builder;
using UmlKit.Renderer.Model;

namespace UmlKit.Builders;

public class MethodFlowDiagramBuilder : IContextDiagramBuilder
{
    public GrouppedSortedTransitionList? Build(string domainName, List<ContextInfo> allContexts, IContextClassifier classifier)
        => throw new NotImplementedException();
}