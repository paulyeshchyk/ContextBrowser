using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;
using UmlKit.DiagramGenerator.Managers;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Renderer;

public abstract class UmlTransitionRendererFlat<P> : IUmlTransitionRenderer<P>
    where P : IUmlParticipant
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IUmlTransitionFactory<P> _factory;
    private readonly INamingProcessor _namingProcessor;
    private readonly IAppOptionsStore _optionsStore;

    public UmlTransitionRendererFlat(IAppLogger<AppLevel> logger, IUmlTransitionFactory<P> factory, INamingProcessor namingProcessor, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _factory = factory;
        _namingProcessor = namingProcessor;
        _optionsStore = optionsStore;
    }

    public abstract UmlDiagram<P> CreateDiagram(DiagramBuilderOptions options);

    public Task<UmlDiagram<P>?> RenderAsync(GrouppedSortedTransitionList? allTransitions, CancellationToken cancellationToken)
    {
        UmlDiagram<P>? diagram = null;
        var activationStack = new RenderContextActivationStack(_logger);
        var plainList = allTransitions?.GetTransitionList();
        if (plainList == null)
        {
            return Task.FromResult(diagram);
        }

        var options = _optionsStore.GetOptions<DiagramBuilderOptions>();

        diagram = CreateDiagram(options);

        Stack<string> callStack = new Stack<string>();

        foreach (var transition in plainList)
        {
            var caller = transition.CallerClassName;
            var callee = transition.CalleeClassName;

            while (callStack.Count != 0 && callStack.Peek() != caller)
            {
                diagram.Deactivate(callStack.Pop());
            }

            var defaultKeywords = new UmlParticipantKeywordsSet()
            {
                Actor = UmlParticipantKeyword.Actor,
                Control = UmlParticipantKeyword.Control,
            };

            // 1. Сначала акторы
            var ctx = new RenderContext<P>(transition, diagram, options, activationStack, _logger);
            if (string.IsNullOrWhiteSpace(ctx.Callee))
            {
                _logger.WriteLog(AppLevel.P_Tran, LogLevel.Err, $"Callee is empty for transition {ctx.Caller}.{ctx.CallerMethod} -> ??.{ctx.CalleeMethod}");
                continue;
            }
            SequenceParticipantsManager.AddParticipants(ctx, defaultKeywords, _namingProcessor);

            // 2. Затем переходы
            if (callStack.Count == 0 || callStack.Peek() != caller)
            {
                SequenceTransitionManager.SystemCall(_factory, options, diagram, caller, transition.CallerMethod ?? "_unknown_caller_method", true);
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
        UmlTransitionRendererFlat<P>.WipeStack(callStack, diagram);
        return Task.FromResult((UmlDiagram<P>?)diagram);
    }

    private static void WipeStack(Stack<string> callStack, UmlDiagram<P> diagram)
    {
        while (callStack.Count > 0)
        {
            diagram.Deactivate(callStack.Pop());
        }
    }
}
