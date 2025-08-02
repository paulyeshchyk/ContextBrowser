using UmlKit.Diagrams;
using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public class MethodFlowDiagramBuilder : IContextDiagramBuilder
{
    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target)
        => throw new NotImplementedException();
}