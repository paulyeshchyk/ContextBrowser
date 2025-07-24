using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public interface IContextDiagramBuilder
{
    string Name { get; }

    bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target);
}
