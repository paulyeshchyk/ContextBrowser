using ContextBrowser.ContextKit.Model;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class MethodOnlyDiagramBuilder : IContextDiagramBuilder
{
    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target)
        => throw new NotImplementedException();
}
