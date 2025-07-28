using ContextBrowser.DiagramFactory.Builders;
using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

namespace ContextBrowser.DiagramFactory;

public static class ContextDiagramFactory
{
    public static IContextDiagramBuilder Transition => _builders["context-transition"];

    public static IContextDiagramBuilder Dependencies => new DependencyDiagramBuilder();

    public static IContextDiagramBuilder MethodsOnly => new MethodOnlyDiagramBuilder();

#warning to be used new ContextTransitionDiagramBuilderOptions() { UseClassAsParticipant = true, UseMethodAsLabel = true }

    private static List<ITransitionDirectionBuilder> DefaultDirectionBuilders =>
        new List<ITransitionDirectionBuilder>() {
        new OutgoingTransitionBuilder(),
        new IncomingTransitionBuilder(),
        new BiDirectionalTransitionBuilder(),
        };

    private static readonly Dictionary<string, IContextDiagramBuilder> _builders =
       new(StringComparer.OrdinalIgnoreCase)
       {
           ["context-transition"] = new ContextTransitionDiagramBuilder(new ContextTransitionDiagramBuilderOptions(), DefaultDirectionBuilders, new RunMethodAsControlParticipantResolver()),
           ["method-flow"] = new MethodFlowDiagramBuilder(),
           ["dependencies"] = new DependencyDiagramBuilder()
       };

    public static IContextDiagramBuilder? Get(string name) =>
        _builders.TryGetValue(name, out var b) ? b : null;

    public static IEnumerable<string> AvailableNames => _builders.Keys;
}
