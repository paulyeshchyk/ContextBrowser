using UmlKit.Diagrams;
using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public interface IContextDiagramBuilder
{
    bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target);
}