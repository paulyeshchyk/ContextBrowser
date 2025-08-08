using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public interface IContextDiagramBuilder
{
    SortedGrouppedTransitionList? Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier);
}