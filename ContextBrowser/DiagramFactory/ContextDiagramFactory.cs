using ContextBrowser.DiagramFactory.Builders;

namespace ContextBrowser.DiagramFactory;

public static class ContextDiagramFactory
{
    public static IContextDiagramBuilder Transition => new ContextTransitionDiagramBuilder(new ContextTransitionDiagramBuilderOptions(), new RunMethodAsControlParticipantResolver());

    public static IContextDiagramBuilder Dependencies => new DependencyDiagramBuilder();

    public static IContextDiagramBuilder MethodsOnly => new MethodOnlyDiagramBuilder();

#warning to be used new ContextTransitionDiagramBuilderOptions() { UseClassAsParticipant = true, UseMethodAsLabel = true }

    private static readonly Dictionary<string, IContextDiagramBuilder> _builders =
       new(StringComparer.OrdinalIgnoreCase)
       {
           ["context-transition"] = new ContextTransitionDiagramBuilder(new ContextTransitionDiagramBuilderOptions(), new RunMethodAsControlParticipantResolver()),
           ["method-flow"] = new MethodFlowDiagramBuilder(),
           ["dependencies"] = new DependencyDiagramBuilder()
       };

    public static IContextDiagramBuilder? Get(string name) =>
        _builders.TryGetValue(name, out var b) ? b : null;

    public static IEnumerable<string> AvailableNames => _builders.Keys;
}
