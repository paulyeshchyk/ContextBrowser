using ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders.Model;
using ContextBrowser.DiagramFactory.Model;
using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public interface ITransitionBuilder
{
    DiagramDirection Direction { get; }

    IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts);
}

public class OutgoingTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.Outgoing;

    public IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        foreach(var ctx in domainMethods)
        {
            foreach(var callee in ctx.References)
            {
                if(callee.ElementType != ContextInfoElementType.method)
                    continue;

                yield return UmlTransitionDtoBuilder.CreateTransition(ctx, callee);
            }
        }
    }
}

public class IncomingTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.Incoming;

    public IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        foreach(var callee in domainMethods)
        {
            foreach(var caller in callee.InvokedBy ?? Enumerable.Empty<ContextInfo>())
            {
                if(caller.ElementType != ContextInfoElementType.method)
                    continue;

                yield return UmlTransitionDtoBuilder.CreateTransition(caller, callee);
            }
        }
    }
}

public class BiDirectionalTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.BiDirectional;

    private readonly OutgoingTransitionBuilder _outgoing = new();
    private readonly IncomingTransitionBuilder _incoming = new();

    public IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        return _outgoing.BuildTransitions(domainMethods, allContexts)
                        .Concat(_incoming.BuildTransitions(domainMethods, allContexts));
    }
}