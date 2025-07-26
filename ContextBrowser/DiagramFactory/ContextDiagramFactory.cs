using ContextBrowser.DiagramFactory.Builders;

namespace ContextBrowser.DiagramFactory;

public static class ContextDiagramFactory
{
    public static IContextDiagramBuilder Transition => new ContextTransitionDiagramBuilder();

    public static IContextDiagramBuilder Dependencies => new DependencyDiagramBuilder();

    public static IContextDiagramBuilder MethodsOnly => new MethodOnlyDiagramBuilder();

    private static readonly Dictionary<string, IContextDiagramBuilder> _builders =
       new(StringComparer.OrdinalIgnoreCase)
       {
           ["context-transition"] = new ContextTransitionDiagramBuilder(new ContextTransitionDiagramBuilderOptions() { UseClassAsParticipant = true, UseMethodAsLabel = true }),
           ["method-flow"] = new MethodFlowDiagramBuilder(),
           ["dependencies"] = new DependencyDiagramBuilder()
       };

    public static IContextDiagramBuilder? Get(string name) =>
        _builders.TryGetValue(name, out var b) ? b : null;

    public static IEnumerable<string> AvailableNames => _builders.Keys;
}
