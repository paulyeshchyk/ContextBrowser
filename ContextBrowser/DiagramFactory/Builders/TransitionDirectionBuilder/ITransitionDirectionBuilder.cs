using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public interface ITransitionDirectionBuilder
{
    DiagramDirection Direction { get; }

    IEnumerable<UmlTransitionDto> BuildTransitions(IEnumerable<ContextInfo> methods);
}

public class OutgoingTransitionBuilder : ITransitionDirectionBuilder
{
    public DiagramDirection Direction => DiagramDirection.Outgoing;

    public IEnumerable<UmlTransitionDto> BuildTransitions(IEnumerable<ContextInfo> methods)
    {
        foreach(var caller in methods)
        {
            foreach(var callee in caller.References)
            {
                if(callee.ElementType != ContextInfoElementType.method)
                    continue;

                yield return UmlTransitionDtoBuilder.CreateTransition(caller, callee);
            }
        }
    }
}

public class IncomingTransitionBuilder : ITransitionDirectionBuilder
{
    public DiagramDirection Direction => DiagramDirection.Incoming;

    public IEnumerable<UmlTransitionDto> BuildTransitions(IEnumerable<ContextInfo> methods)
    {
        foreach(var callee in methods)
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

public class BiDirectionalTransitionBuilder : ITransitionDirectionBuilder
{
    public DiagramDirection Direction => DiagramDirection.BiDirectional;

    private readonly OutgoingTransitionBuilder _outgoing = new();
    private readonly IncomingTransitionBuilder _incoming = new();

    public IEnumerable<UmlTransitionDto> BuildTransitions(IEnumerable<ContextInfo> methods)
    {
        return _outgoing.BuildTransitions(methods)
                        .Concat(_incoming.BuildTransitions(methods));
    }
}