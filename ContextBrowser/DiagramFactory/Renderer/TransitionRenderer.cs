using ContextBrowser.DiagramFactory.Renderer.Model;
using ContextBrowser.Infrastructure;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Diagrams;
using UmlKit.Extensions;
using UmlKit.Model;

namespace ContextBrowser.DiagramFactory.Renderer;

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
        var activationStack = new ActivationStack(_onWriteLog);
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
        //ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render Single Transition [{ctx.Caller}.{ctx.CallerMethod} -> {ctx.Callee}.{ctx.CalleeMethod}]", LogLevelNode.Start);

        AddParticipants(ctx, UmlParticipantKeyword.Actor);

        _ = ActivateCaller(ctx);

        _ = RenderCallerToStep1(ctx);

        _ = RenderStep1ToStep2(ctx);

        _ = RenderStep2ToStep1Return(ctx);

        _ = RenderStep1ToCallerDone(ctx);

        _ = DeactivateCaller(ctx);


        //ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private static bool RenderCallerToStep1(RenderContext ctx)
    {
        //TODO: extra convert
        string? from = ctx.Caller?.AlphanumericOnly();
        string? to = ctx.Step1?.AlphanumericOnly();
        string? label = ctx.CallerMethod;

        if(from?.Equals(to) ?? false)
            ctx.Diagram.AddSelfCallBreak(from);

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render CallerToStep1 transition [{from} -> {to}]:<{label}>");
        ctx.Diagram.AddTransition(from, to, isAsync: ctx.Options.UseAsync, label);
        var pushed = TryActivate(ctx, to);
        return pushed;
    }

    private static bool RenderStep1ToStep2(RenderContext ctx)
    {
        string? from = ctx.Step1;
        string? to = ctx.Step2;
        string? label = ctx.CalleeMethod;

        if(string.IsNullOrWhiteSpace(to))
            return false;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render transition S1S2 [{from} -> {to}]:<{label}>");
        ctx.Diagram.AddTransition(from, to, isAsync: ctx.Options.UseAsync, label);
        var pushed = TryActivate(ctx, to);
        return pushed;
    }

    private static bool RenderStep2ToStep1Return(RenderContext ctx)
    {
        string? from = ctx.Step2;
        string? to = ctx.Step1;

        if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return false;
        }

        if(ctx.Options.UseReturn)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render transition S2S1 [{from} -> {to}]:<return>");
            ctx.Diagram.AddTransition(from, to, isAsync: ctx.Options.UseAsync, "return");
        }

        var poped = TryDeactivate(ctx, from);
        return poped;
    }

    private static bool RenderStep1ToCallerDone(RenderContext ctx)
    {
        string? from = ctx.Step1;
        string? to = ctx.Caller;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render Step1ToCallerDone transition [{from} -> {to}]:<done>");
        ctx.Diagram.AddTransition(from, to, isAsync: ctx.Options.UseAsync, "done");
        var poped = TryDeactivate(ctx, from);
        return poped;
    }

    private static void AddParticipants(RenderContext ctx, UmlParticipantKeyword defaultParticipantKeyword)
    {
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Adding paticipants for context: {ctx.RunContext}", LogLevelNode.Start);
        var participants = new[] { ctx.Transition.CallerClassName, ctx.Transition.CalleeClassName, ctx.Transition.RunContext }.Where(x => !string.IsNullOrWhiteSpace(x)).Select(s => s!);
        foreach(var p in participants)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Adding participant [{p}][{ctx.Transition.CallerClassName}.{ctx.Transition.CallerMethod} -> {ctx.Transition.CalleeClassName}.{ctx.Transition.CalleeMethod}]");

            ctx.Diagram.AddParticipant(p, p.AlphanumericOnly(), defaultParticipantKeyword);
        }
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private static bool ActivateCaller(RenderContext ctx)
    {
        if(ctx.Options.UseSelfCallContinuation)
        {
            ctx.Diagram.AddSelfCallContinuation(ctx.Caller);
        }

        if(ctx.Options.UseSelfCallContinuation && ctx.Options.UseActivation)
            ctx.Diagram.Activate(ctx.Caller, isSystemCall: false);

        if(ctx.Options.UseActivation)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Stack activate {ctx.Caller}");
            ctx.ActivationStack.TryPush(ctx.Caller);
        }
        return ctx.Options.UseActivation;
    }

    private static bool DeactivateCaller(RenderContext ctx)
    {
        if(!ctx.Options.UseActivation)
        {
            return false;
        }
        return TryDeactivate(ctx, ctx.Caller);
    }

    private static bool TryActivate(RenderContext ctx, string? participant)
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(participant))
            return false;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Stack activate {participant}");
        ctx.Diagram.Activate(participant, isSystemCall: false);
        ctx.ActivationStack.TryPush(participant);
        return true;
    }

    private static bool TryDeactivate(RenderContext ctx, string? participant)
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(participant))
            return false;

        var poped = ctx.ActivationStack.TryPeek2(out var top);
        if(poped)
        {
            if(top == participant)
            {
                ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Render deactivate {participant}");
                ctx.Diagram.Deactivate(ctx.ActivationStack.Pop());
            }
            else
            {
                ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"Stack deactivate {participant} - incorrect participant: expected {top}");
            }
        }
        else
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"Stack deactivate {participant}");
        }
        return poped;
    }
}
