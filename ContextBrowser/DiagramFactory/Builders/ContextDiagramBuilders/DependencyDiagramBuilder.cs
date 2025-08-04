using ContextBrowser.DiagramFactory.Model;
using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public class DependencyDiagramBuilder : IContextDiagramBuilder
{
    public SortedList<int, UmlTransitionDto>? Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier)
        => throw new NotImplementedException();
}