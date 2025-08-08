using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public class DependencyDiagramBuilder : IContextDiagramBuilder
{
    public SortedGrouppedTransitionList? Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier)
        => throw new NotImplementedException();

    SortedGrouppedTransitionList? IContextDiagramBuilder.Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier)
    {
        throw new NotImplementedException();
    }
}