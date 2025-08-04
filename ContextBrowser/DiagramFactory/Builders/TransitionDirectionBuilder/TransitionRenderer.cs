using ContextBrowser.DiagramFactory.Model;
using ContextBrowser.Infrastructure;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Diagrams;
using UmlKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public class TransitionRenderer
{
    private readonly OnWriteLog? _onWriteLog;

    public TransitionRenderer(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    public bool RenderAllTransitions(UmlDiagram diagram, SortedList<int, UmlTransitionDto>? allTransitions, AppOptions _options, string domain)
    {
        if((allTransitions == null) || !allTransitions.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"No transitions provided for [{domain}]");
            return false;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Rendering transitions for [{domain}]", LogLevelNode.Start);
        var activationStack = new Stack<string>();
        var seenTransitions = new HashSet<string>();

        foreach(var transition in allTransitions.OrderBy(t => t.Key).Select(t => t.Value))
        {
            var key = $"{transition.CallerClassName}.{transition.CallerMethod}→{transition.CalleeClassName}.{transition.CalleeMethod}";

            if(!seenTransitions.Add(key) && _options.roslynCodeparserOptions.ShowForeignInstancies)
            {
                diagram.AddNoteOver(transition.CallerClassName, $"duplicate run call on {transition.CalleeClassName}");
                diagram.AddNoteOver(transition.CalleeClassName, $"duplicate run call on {transition.CallerClassName}");
                continue;
            }

            var ctx = new RenderContext(transition, diagram, _options.contextTransitionDiagramBuilderOptions, activationStack, _onWriteLog);
            RenderFullTransition(ctx);
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return true;
    }


    private static void RenderFullTransition(RenderContext ctx)
    {
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render Transition [{ctx.Caller}.{ctx.CallerMethod} -> {ctx.Callee}.{ctx.CalleeMethod}]", LogLevelNode.Start);

        AddParticipants(ctx, UmlParticipantKeyword.Actor);
        MaybeAddSelfCallContinuation(ctx);

        RenderCallerToStep1(ctx);
        RenderStep1ToStep2(ctx);
        RenderStep2ToStep1Return(ctx);
        RenderStep1ToCallerDone(ctx);

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private static void RenderCallerToStep1(RenderContext ctx)
    {
        string? from = ctx.Caller;
        string? to = ctx.Step1;
        string? label = ctx.CallerMethod;

        if(from == to)
            ctx.Diagram.AddSelfCallBreak(from);

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render transition [{from} -> {to}]:<{label}>");
        ctx.Diagram.AddTransition(from, to, label);
        TryActivate(ctx, to);
    }

    private static void RenderStep1ToStep2(RenderContext ctx)
    {
        string? from = ctx.Step1;
        string? to = ctx.Step2;
        string? label = ctx.CalleeMethod;

        if(string.IsNullOrWhiteSpace(to))
            return;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render transition S1S2 [{from} -> {to}]:<{label}>");
        ctx.Diagram.AddTransition(from, to, label);
        TryActivate(ctx, to);
    }

    private static void RenderStep2ToStep1Return(RenderContext ctx)
    {
        string? from = ctx.Step2;
        string? to = ctx.Step1;

        if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            return;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render transition S2S1 [{from} -> {to}]:<return>");
        ctx.Diagram.AddTransition(from, to, "return");
        TryDeactivate(ctx, from);
    }

    private static void RenderStep1ToCallerDone(RenderContext ctx)
    {
        string? from = ctx.Step1;
        string? to = ctx.Caller;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render transition [{from} -> {to}]:<done>");
        ctx.Diagram.AddTransition(from, to, "done");
        TryDeactivate(ctx, from);
        TryDeactivate(ctx, to);
    }

    private static void AddParticipants(RenderContext ctx, UmlParticipantKeyword defaultParticipantKeyword)
    {
        foreach(var p in new[] { ctx.Caller, ctx.Callee, ctx.RunContext }.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render participant [{ctx.RunContext}][{ctx.Caller}.{ctx.CallerMethod} -> {ctx.Callee}.{ctx.CalleeMethod}]");
            ctx.Diagram.AddParticipant(p!, defaultParticipantKeyword);
        }
    }

    private static void MaybeAddSelfCallContinuation(RenderContext ctx)
    {
        if(ctx.Options.UseActivation && ctx.Options.UseSelfCallContinuation)
        {
            ctx.Diagram.AddSelfCallContinuation(ctx.Caller);
            ctx.ActivationStack.Push(ctx.Caller);
        }
    }

    private static void TryActivate(RenderContext ctx, string? participant)
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(participant))
            return;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render activate {participant}");
        ctx.Diagram.Activate(participant);
        ctx.ActivationStack.Push(participant);
    }

    private static void TryDeactivate(RenderContext ctx, string? participant)
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(participant))
            return;

        if(ctx.ActivationStack.TryPeek(out var top) && top == participant)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render deactivate {participant}");
            ctx.Diagram.Deactivate(ctx.ActivationStack.Pop());
        }
    }

    private sealed class RenderContext
    {
        private UmlTransitionDto T { get; }

        public UmlDiagram Diagram { get; }

        public ContextTransitionDiagramBuilderOptions Options { get; }

        public Stack<string> ActivationStack { get; }

        public OnWriteLog? Log { get; }

        public string? Caller => T.CallerClassName;

        public string? Callee => T.CalleeClassName;

        public string? RunContext => T.RunContext;

        public string? CallerMethod => T.CallerMethod;

        public string? CalleeMethod => T.CalleeMethod;

        public string? Step1 => !string.IsNullOrWhiteSpace(RunContext) ? RunContext : Callee;

        public string? Step2 => !string.IsNullOrWhiteSpace(RunContext) ? Callee : null;

        public RenderContext(
            UmlTransitionDto t,
            UmlDiagram diagram,
            ContextTransitionDiagramBuilderOptions options,
            Stack<string> activationStack,
            OnWriteLog? log)
        {
            T = t;
            Diagram = diagram;
            Options = options;
            ActivationStack = activationStack;
            Log = log;
        }
    }
}

