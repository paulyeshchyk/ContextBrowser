using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public interface IContextDiagramBuilder
{
    GrouppedSortedTransitionList? Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier);
}