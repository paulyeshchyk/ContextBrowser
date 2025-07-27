using ContextBrowser.ContextKit.Model;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class MethodFlowDiagramBuilder : IContextDiagramBuilder
{
    public string Name => "Method";

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target)
        => throw new NotImplementedException();
}