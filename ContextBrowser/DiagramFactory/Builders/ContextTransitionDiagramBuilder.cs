using ContextBrowser.ContextKit.Model;
using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly ContextTransitionDiagramBuilderOptions _options;
    private readonly List<ITransitionBuilder> _transitionBuilders;

    public ContextTransitionDiagramBuilder(ContextTransitionDiagramBuilderOptions? options, IEnumerable<ITransitionBuilder> transitionBuilders)
    {
        _options = options ?? new ContextTransitionDiagramBuilderOptions(detailLevel: DiagramDetailLevel.Full, direction: DiagramDirection.BiDirectional, defaultParticipantKeyword: UmlKit.Model.UmlParticipantKeyword.Actor, useMethodAsParticipant: false);
        _transitionBuilders = transitionBuilders.ToList();
    }

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram diagram)
    {
        var methods = allContexts
            .Where(ctx =>
                ctx.ElementType == ContextInfoElementType.method &&
                ctx.Domains.Contains(domainName) &&
                classifier.HasActionAndDomain(ctx))
            .ToList();

        if(!methods.Any())
        {
            Console.WriteLine($"[MISS]: No methods for domain '{domainName}'");
            return false;
        }

        var allTransitions = new HashSet<UmlTransitionDto>();

        foreach(var builder in _transitionBuilders)
        {
            var transitions = builder.BuildTransitions(methods, allContexts);
            foreach(var t in transitions)
                allTransitions.Add(t);
        }

        if(!allTransitions.Any())
            return false;

        foreach(var t in allTransitions)
        {
            TransitionRenderer.RenderFullTransition(diagram, t, _options);
        }

        TransitionRenderer.FinalizeDiagram(diagram);

        return true;
    }
}
