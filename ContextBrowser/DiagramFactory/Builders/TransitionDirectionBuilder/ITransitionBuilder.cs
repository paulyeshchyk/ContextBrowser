using ContextBrowser.DiagramFactory.Model;
using ContextKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public interface ITransitionBuilder
{
    DiagramDirection Direction { get; }

    IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts);
}
