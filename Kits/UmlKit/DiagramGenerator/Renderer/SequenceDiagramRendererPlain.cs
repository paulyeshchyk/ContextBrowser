using ContextBrowserKit.Log;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.DiagramGenerator.Managers;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Renderer;

public class SequenceDiagramRendererPlain<P> : ISequenceDiagramRenderer<P>
    where P : IUmlParticipant
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly DiagramBuilderOptions _options;
    private readonly IUmlTransitionFactory<P> _factory;

    public SequenceDiagramRendererPlain(OnWriteLog? onWriteLog, DiagramBuilderOptions options, IUmlTransitionFactory<P> factory)
    {
        _onWriteLog = onWriteLog;
        _options = options;
        _factory = factory;
    }

    public void Render(UmlDiagram<P> diagram, GrouppedSortedTransitionList? allTransitions)
    {
        var activationStack = new RenderContextActivationStack(_onWriteLog);
        var plainList = allTransitions?.GetTransitionList();
        if(plainList == null)
        {
            return;
        }

        Stack<string> callStack = new Stack<string>();

        foreach(var transition in plainList)
        {
            var caller = transition.CallerClassName;
            var callee = transition.CalleeClassName;

            while(callStack.Any() && callStack.Peek() != caller)
            {
                diagram.Deactivate(callStack.Pop());
            }

            if(!callStack.Any() || callStack.Peek() != caller)
            {
                SequenceTransitionManager.SystemCall(_factory, _options, diagram, caller, transition.CallerMethod ?? "_unknown_caller_method", true);
                callStack.Push(caller);
            }

            var ctx = new RenderContext<P>(transition, diagram, _options, activationStack, _onWriteLog);
            SequenceParticipantsManager.AddParticipants(ctx, UmlParticipantKeyword.Actor);

            _ = SequenceActivationManager.RenderActivateCaller(ctx);
            _ = SequenceActivationManager.RenderActivateCallee(ctx);

            _ = SequenceInvocationManager.RenderCalleeInvocation(ctx);

            _ = SequenceActivationManager.RenderDeactivateCallee(ctx);
            _ = SequenceActivationManager.RenderDeactivateCaller(ctx);


            if(!string.IsNullOrWhiteSpace(callee) && !callStack.Contains(callee))
            {
                callStack.Push(callee);
            }
        }
        WipeStack(callStack, diagram);
    }

    private void WipeStack(Stack<string> callStack, UmlDiagram<P> diagram)
    {
        while(callStack.Count > 0)
        {
            diagram.Deactivate(callStack.Pop());
        }
    }
}
