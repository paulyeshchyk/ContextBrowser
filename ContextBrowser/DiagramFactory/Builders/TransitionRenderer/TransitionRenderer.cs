using ContextBrowser.DiagramFactory.Model;
using ContextBrowser.Infrastructure;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Diagrams;
using UmlKit.Extensions;
using UmlKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.TransitionRenderer;

public class TransitionRenderer
{
    protected readonly OnWriteLog? _onWriteLog;
    private readonly AppOptions _options;

    public TransitionRenderer(AppOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
    }

    public bool RenderDiagramTransitions(UmlDiagram diagram, SortedList<int, UmlTransitionDto>? allTransitions, string domain)
    {
        if(allTransitions == null || !allTransitions.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"No transitions provided for [{domain}]");
            return false;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Rendering Diagram transitions for [{domain}]", LogLevelNode.Start);
        var activationStack = new Stack<string>();
        var seenTransitions = new HashSet<string>();
        var transitonList = allTransitions.OrderBy(t => t.Key).Select(t => t.Value);
        foreach(var transition in transitonList)
        {
            var key = $"{transition.CallerClassName}.{transition.CallerMethod}->{transition.CalleeClassName}.{transition.CalleeMethod}";

            if(!seenTransitions.Add(key) && _options.roslynCodeparserOptions.ShowForeignInstancies)
            {
                diagram.AddNoteOver(transition.CallerClassName ?? "unknown callee", $"duplicate run call on {transition.CalleeClassName}");
                diagram.AddNoteOver(transition.CalleeClassName ?? "unknown callee", $"duplicate run call on {transition.CallerClassName}");
                continue;
            }

            var ctx = new RenderContext(transition, diagram, _options.contextTransitionDiagramBuilderOptions, activationStack, _onWriteLog);
            RenderSingleTransition(ctx);
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return true;
    }

    internal static void RenderSingleTransition(RenderContext ctx)
    {
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render Single Transition [{ctx.Caller}.{ctx.CallerMethod} -> {ctx.Callee}.{ctx.CalleeMethod}]", LogLevelNode.Start);

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
        //TODO: extra convert
        string? from = ctx.Caller?.AlphanumericOnly();
        string? to = ctx.Step1?.AlphanumericOnly();
        string? label = ctx.CallerMethod;

        if(from?.Equals(to) ?? false)
            ctx.Diagram.AddSelfCallBreak(from);

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render CallerToStep1 transition [{from} -> {to}]:<{label}>");
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

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render Step1ToCallerDone transition [{from} -> {to}]:<done>");
        ctx.Diagram.AddTransition(from, to, "done");
        TryDeactivate(ctx, from);
        TryDeactivate(ctx, to);
    }

    private static void AddParticipants(RenderContext ctx, UmlParticipantKeyword defaultParticipantKeyword)
    {
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Adding paticipants for context: {ctx.RunContext}", LogLevelNode.Start);
        foreach(var p in new[] { ctx.Transition.CallerClassName, ctx.Transition.CalleeClassName, ctx.Transition.RunContext }.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            if(string.IsNullOrEmpty(p))
                continue;
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg,
                $"Adding participant [{p}][{ctx.Transition.CallerClassName}.{ctx.Transition.CallerMethod} -> {ctx.Transition.CalleeClassName}.{ctx.Transition.CalleeMethod}]");

            ctx.Diagram.AddParticipant(p, p.AlphanumericOnly(), defaultParticipantKeyword);
        }
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
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
}
