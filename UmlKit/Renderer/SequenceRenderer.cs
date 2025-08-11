using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Diagrams;
using UmlKit.Extensions;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.Renderer.Builder;
using UmlKit.Renderer.Model;

namespace UmlKit.Renderer;

public class SequenceRenderer<P>
    where P : IUmlParticipant
{
    protected readonly OnWriteLog? _onWriteLog;
    private readonly ContextTransitionDiagramBuilderOptions _options;
    private readonly IUmlTransitionFactory<P> _factory;

    public SequenceRenderer(ContextTransitionDiagramBuilderOptions options, OnWriteLog? onWriteLog, IUmlTransitionFactory<P> factory)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _factory = factory;
    }

    public bool RenderDiagramTransitions(UmlDiagram<P> diagram, GrouppedSortedTransitionList? allTransitions, string domain)
    {
        if(allTransitions == null || !allTransitions.HasTransitions())
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"No transitions provided for [{domain}]");
            return false;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Rendering Diagram transitions for [{domain}]", LogLevelNode.Start);

        var activationStack = new RenderContextActivationStack(_onWriteLog);
        var callStack = new Stack<string>();
        string? pendingActivation = null;

        // Используем упорядоченный список
        var transitionList = allTransitions.GetTransitionList();

        string? lastCallee = null;

        foreach(var transition in transitionList)
        {
            var caller = transition.CallerClassName;
            var callee = transition.CalleeClassName;

            // --- Восстановление контекста ---
            // Если предыдущий callee совпадает с текущим caller, 
            // то мы, скорее всего, продолжаем вложенный сценарий
            if(lastCallee != null && lastCallee.Equals(caller) && !callStack.Contains(caller))
            {
                SystemCall(caller, diagram, "callee == caller");
                pendingActivation = caller;
                callStack.Push(caller);
            }

            // --- Caller ---
            if(callStack.Count > 0)
            {
                if(callStack.Peek() != caller)
                {
                    if(callStack.Contains(caller))
                    {
                        while(callStack.Count > 0 && callStack.Peek() != caller)
                        {
                            var toClose = callStack.Pop();
                            if(pendingActivation == toClose)
                                pendingActivation = null;
                            else
                            {
                                diagram.Deactivate(toClose);
                            }
                        }
                    }
                    else
                    {
                        while(callStack.Count > 0)
                        {
                            var toClose = callStack.Pop();
                            if(pendingActivation == toClose)
                                pendingActivation = null;
                            else
                                diagram.Deactivate(toClose);
                        }

                        SystemCall(caller, diagram, "push caller");

                        pendingActivation = caller;
                        callStack.Push(caller);
                    }
                }
            }
            else
            {
                SystemCall(caller, diagram, transition.CallerMethod ?? "_unknown_caller_method", true);

                pendingActivation = caller;
                callStack.Push(caller);
            }

            // --- Переход ---
            if(pendingActivation != null)
            {
                diagram.Activate(pendingActivation, reason: string.Empty, softActivation: false);//true
                pendingActivation = null;
            }

            var ctx = new RenderContext<P>(transition, diagram, _options, activationStack, _onWriteLog);
            RenderSingleTransition(ctx);

            // --- Callee ---
            if(!string.IsNullOrWhiteSpace(callee))
            {
                if(!callStack.Contains(callee))
                {
                    SystemCall(callee, diagram, "diagram activation issue fix");

                    pendingActivation = callee;
                    callStack.Push(callee);
                }
            }

            lastCallee = callee;
        }

        // --- Закрытие ---
        while(callStack.Count > 0)
        {
            var toClose = callStack.Pop();
            if(pendingActivation == toClose)
                pendingActivation = null;
            else
                diagram.Deactivate(toClose);
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return true;
    }

    private void SystemCall(string caller, UmlDiagram<P> diagram, string reason, bool visibleWithoutActivation = false)
    {
        if(_options.UseActivation || visibleWithoutActivation)
        {
            var from = _factory.CreateTransitionObject(string.Empty);
            var to = _factory.CreateTransitionObject(caller);
            var systemCall = _factory.CreateTransition(from, to, reason);
            diagram.AddTransition(systemCall);
        }
    }


    internal static void RenderSingleTransition<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        AddParticipants(ctx, UmlParticipantKeyword.Actor);

        _ = RenderActivateCaller(ctx);

        _ = RenderActivateCallee(ctx);

        _ = RenderActivateCalleeInvocation(ctx);

        _ = RenderDeactivateCalleeInvocation(ctx);

        _ = RenderDeactivateCallee(ctx);

        _ = RenderDeactivateCaller(ctx);
    }

    private static bool RenderActivateCallee<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        //TODO: extra convert
        string? from = ctx.Caller?.AlphanumericOnly();
        string? to = ctx.Step1?.AlphanumericOnly();
        string? label = ctx.CalleeMethod;

        //if (from?.Equals(to) ?? false)
        //    ctx.Diagram.AddCallbreakNote(from);


        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render CallerToStep1 transition [{from} -> {to}]:<{label}>");

        var fromParticipant = ctx.Diagram.AddParticipant(from);
        var toParticipant = ctx.Diagram.AddParticipant(to);
        ctx.Diagram.AddTransition(fromParticipant, toParticipant, isAsync: ctx.Options.UseAsync, label);

        ctx.Diagram.Activate(from, to, reason: "act1vate callee", true);


        //"Outside"
        var pushed = TryActivate(ctx, source: from, destination: to, $"EXEC {ctx.CalleeMethod}");
        return pushed;
    }

    private static bool RenderActivateCalleeInvocation<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        string? from = ctx.Step1;
        string? to = ctx.Step2;
        string? label = ctx.CalleeMethod;

        if(string.IsNullOrWhiteSpace(to))
            return false;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render transition S1S2 [{from} -> {to}]:<{label}>");

        ctx.Diagram.Activate(from, to, reason: "act1vate invocation", true);

        var fromParticipant = ctx.Diagram.AddParticipant(from);
        var toParticipant = ctx.Diagram.AddParticipant(from);
        ctx.Diagram.AddTransition(fromParticipant, toParticipant, isAsync: ctx.Options.UseAsync, label);
        var pushed = TryActivate(ctx, source: from, destination: to, "Callee invocation");
        return pushed;
    }

    private static bool RenderDeactivateCalleeInvocation<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        string? from = ctx.Step2;
        string? to = ctx.Step1;

        if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return false;
        }

        ctx.Diagram.Activate(from, to, reason: "deact1vate invocation", true);

        if(ctx.Options.UseReturn)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render transition S2S1 [{from} -> {to}]:<return>");
            var fromParticipant = ctx.Diagram.AddParticipant(from);
            var toParticipant = ctx.Diagram.AddParticipant(from);
            ctx.Diagram.AddTransition(fromParticipant, toParticipant, isAsync: ctx.Options.UseAsync, "return");
        }

        var poped = TryDeactivate(ctx, from);
        return poped;
    }

    private static bool RenderDeactivateCallee<T>(RenderContext<T> ctx, string reason = "done")
        where T : IUmlParticipant
    {
        string? from = ctx.Step1;
        string? to = ctx.Caller;

        //ctx.Diagram.Activate(from, to, reason: reason, true);

        var fromParticipant = ctx.Diagram.AddParticipant(from);
        var toParticipant = ctx.Diagram.AddParticipant(to);

        ctx.Diagram.AddTransition(fromParticipant, toParticipant, isAsync: ctx.Options.UseAsync, reason);

        var poped = TryDeactivate(ctx, from);
        return poped;
    }

    private static void AddParticipants<T>(RenderContext<T> ctx, UmlParticipantKeyword defaultParticipantKeyword)
        where T : IUmlParticipant
    {
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Adding paticipants for context: {ctx.RunContext}", LogLevelNode.Start);
        var participants = new[] { ctx.Transition.CallerClassName, ctx.Transition.CalleeClassName, ctx.Transition.RunContext }.Where(x => !string.IsNullOrWhiteSpace(x)).Select(s => s!);
        foreach(var p in participants)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Adding participant [{p}][{ctx.Transition.CallerClassName}.{ctx.Transition.CallerMethod} -> {ctx.Transition.CalleeClassName}.{ctx.Transition.CalleeMethod}]");

            ctx.Diagram.AddParticipant(p, keyword: defaultParticipantKeyword);
        }
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private static bool RenderActivateCaller<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        if(ctx.Options.UseSelfCallContinuation)
        {
            ctx.Diagram.AddSelfCallContinuation(ctx.Caller, ctx.CallerMethod ?? "_unknown_caller_method");
        }

        if(ctx.Options.UseSelfCallContinuation && ctx.Options.UseActivation)
            ctx.Diagram.Activate(ctx.Caller, reason: string.Empty, softActivation: true);

        if(ctx.Options.UseActivation)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Stack activate {ctx.Caller}");
            ctx.ActivationStack.TryPush(ctx.Caller);
        }
        return ctx.Options.UseActivation;
    }

    private static bool RenderDeactivateCaller<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        if(!ctx.Options.UseActivation)
        {
            return false;
        }
        return TryDeactivate(ctx, ctx.Caller);
    }

    private static bool TryActivate<T>(RenderContext<T> ctx, string source, string? destination, string reason)
        where T : IUmlParticipant
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(destination))
            return false;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Stack activate {destination}");
        ctx.Diagram.Activate(source: source, destination, reason: reason, softActivation: false);
        ctx.ActivationStack.TryPush(destination);
        return true;
    }

    private static bool TryDeactivate<T>(RenderContext<T> ctx, string? participant)
        where T : IUmlParticipant
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(participant))
            return false;

        var poped = ctx.ActivationStack.TryPeek2(out var top);
        if(poped)
        {
            if(top == participant)
            {
                ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render deactivate {participant}");
                var item = ctx.ActivationStack.Pop();
                ctx.Diagram.Deactivate(item);
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
