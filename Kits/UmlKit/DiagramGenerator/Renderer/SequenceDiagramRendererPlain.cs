using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
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
    private readonly IAppLogger<AppLevel> _logger;
    private readonly DiagramBuilderOptions _options;
    private readonly IUmlTransitionFactory<P> _factory;

    public SequenceDiagramRendererPlain(IAppLogger<AppLevel> logger, DiagramBuilderOptions options, IUmlTransitionFactory<P> factory)
    {
        _logger = logger;
        _options = options;
        _factory = factory;
    }

    public void Render(UmlDiagram<P> diagram, GrouppedSortedTransitionList? allTransitions)
    {
        var activationStack = new RenderContextActivationStack(_logger);
        var plainList = allTransitions?.GetTransitionList();
        if (plainList == null)
        {
            return;
        }

        Stack<string> callStack = new Stack<string>();

        foreach (var transition in plainList)
        {
            var caller = transition.CallerClassName;
            var callee = transition.CalleeClassName;

            while (callStack.Any() && callStack.Peek() != caller)
            {
                diagram.Deactivate(callStack.Pop());
            }

            var defaultKeywords = new UmlParticipantKeywordsSet()
            {
                Actor = UmlParticipantKeyword.Actor,
                Control = UmlParticipantKeyword.Control,
            };

            // 1. Сначала акторы
            var ctx = new RenderContext<P>(transition, diagram, _options, activationStack, _logger);
            if (string.IsNullOrWhiteSpace(ctx.Callee))
            {
#warning to be fixed asap
                _logger.WriteLog(AppLevel.P_Tran, LogLevel.Err, $"Callee is empty for transition {ctx.Caller}.{ctx.CallerMethod} -> ??.{ctx.CalleeMethod}");
                continue;
            }
            SequenceParticipantsManager.AddParticipants(ctx, defaultKeywords);

            // 2. Затем переходы
            if (!callStack.Any() || callStack.Peek() != caller)
            {
                SequenceTransitionManager.SystemCall(_factory, _options, diagram, caller, transition.CallerMethod ?? "_unknown_caller_method", true);
                callStack.Push(caller);
            }

            // 3. Позже всё остальное

            _ = SequenceActivationManager.RenderActivateCaller(ctx);
            _ = SequenceActivationManager.RenderActivateCallee(ctx);

            _ = SequenceInvocationManager.RenderCalleeInvocation(ctx);

            _ = SequenceActivationManager.RenderDeactivateCallee(ctx);
            _ = SequenceActivationManager.RenderDeactivateCaller(ctx);

            if (!string.IsNullOrWhiteSpace(callee) && !callStack.Contains(callee))
            {
                callStack.Push(callee);
            }
        }
        WipeStack(callStack, diagram);
    }

    private void WipeStack(Stack<string> callStack, UmlDiagram<P> diagram)
    {
        while (callStack.Count > 0)
        {
            diagram.Deactivate(callStack.Pop());
        }
    }
}
