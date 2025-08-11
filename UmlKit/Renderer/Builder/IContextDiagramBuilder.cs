using ContextKit.Model;
using UmlKit.Renderer.Model;

namespace UmlKit.Renderer.Builder;

public interface IContextDiagramBuilder
{
    GrouppedSortedTransitionList? Build(string domainName, List<ContextInfo> allContexts, IContextClassifier classifier);
}