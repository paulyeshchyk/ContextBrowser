using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Renderer.Sequence;

public static class SequenceRendererUtils
{
    internal static void SystemCall<P>(IUmlTransitionFactory<P> factory, DiagramBuilderOptions options, UmlDiagram<P> diagram, string caller, string reason, bool visibleWithoutActivation = false)
        where P : IUmlParticipant
    {
        if(options.UseActivation || visibleWithoutActivation)
        {
            var from = factory.CreateTransitionObject(string.Empty);
            var to = factory.CreateTransitionObject(caller);
            var systemCall = factory.CreateTransition(from, to, options.Debug ? $"SYSTEM CALL [ {reason} ]" : reason);
            diagram.AddTransition(systemCall);
        }
    }

    internal static void AddParticipants<T>(RenderContext<T> ctx, UmlParticipantKeyword defaultParticipantKeyword)
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

    internal static bool RenderActivateCaller<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        if(!ctx.Options.UseActivation)
        {
            return false;
        }

        if(ctx.Options.Indication.UseSelfCallContinuation)
        {
            ctx.Diagram.Activate(source: ctx.Caller, ctx.Caller, reason: ctx.Options.Debug ? $"CALLER ACTIVATE [ {ctx.CalleeMethod} ]" : $"{ctx.CalleeMethod}", softActivation: false);
        }
        ctx.ActivationStack.TryPush(ctx.Caller);
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Stack activate {ctx.Caller}");

        return true;
    }

    internal static bool RenderDeactivateCaller<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        if(!ctx.Options.UseActivation)
        {
            return false;
        }
        return TryDeactivate(ctx, ctx.Caller);
    }

    internal static bool TryActivate<T>(RenderContext<T> ctx, string source, string? destination, string reason)
        where T : IUmlParticipant
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(destination))
            return false;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Stack activate {destination}");
        ctx.Diagram.Activate(source: source, destination, reason: ctx.Options.Debug ? $"INVOCATION ACTIVATE [ {reason} ]" : $"{reason}", softActivation: false);
        ctx.ActivationStack.TryPush(destination);
        return true;
    }

    internal static bool TryDeactivate<T>(RenderContext<T> ctx, string? participant)
        where T : IUmlParticipant
    {
        if(!ctx.Options.UseActivation || string.IsNullOrWhiteSpace(participant))
            return false;

        // Проверяем, что участник на вершине стека совпадает с ожидаемым.
        // Если да, деактивируем.
        if(ctx.ActivationStack.TryPeek(out var top) && top == participant)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render deactivate {participant}");
            var item = ctx.ActivationStack.Pop();
            ctx.Diagram.Deactivate(item);
            return true;
        }

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"Stack deactivate {participant} - incorrect or empty stack.");
        return false;
    }

    internal static bool RenderActivateCallee<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        string? from = ctx.Caller?.AlphanumericOnly();
        string? to = ctx.Step1?.AlphanumericOnly();
        string? label = ctx.CalleeMethod;

        if(!ctx.Options.Indication.UseCalleeActivation)
        {
            return true;
        }

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render CallerToStep1 transition [{from} -> {to}]:<{label}>");

        var fromParticipant = ctx.Diagram.AddParticipant(from);
        var toParticipant = ctx.Diagram.AddParticipant(to);

        if(!ctx.Options.UseActivation)
        {
            ctx.Diagram.AddTransition(fromParticipant, toParticipant, isAsync: ctx.Options.Indication.UseAsync, label);
            return true;
        }

        return TryActivate(ctx, source: from, destination: to, ctx.Options.Debug ? $"CALLER CALLEE ACTIVATE [ {ctx.CalleeMethod} ]" : $"{ctx.CalleeMethod}");
    }

    internal static bool RenderActivateCalleeInvocation<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        string? from = ctx.Step1;
        string? to = ctx.Step2;
        string? label = ctx.CalleeMethod;
        if(string.IsNullOrWhiteSpace(to))
            return false;

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render transition S1S2 [{from} -> {to}]:<{label}>");

        if(ctx.Options.Indication.UseCalleeInvocation)
        {
            return TryActivate(ctx, source: from, destination: to, ctx.Options.Debug ? $"CALLEE INVOCATION ACTIVATE [ {label} ]" : $"{label}");
        }

        return true;
    }

    internal static bool RenderDeactivateCalleeInvocation<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        string? from = ctx.Step2;
        string? to = ctx.Step1;

        if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return false;
        }

        if(ctx.Options.Indication.UseReturn)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render transition S2S1 [{from} -> {to}]:<return>");
            var fromParticipant = ctx.Diagram.AddParticipant(from);
            var toParticipant = ctx.Diagram.AddParticipant(from);
            ctx.Diagram.AddTransition(fromParticipant, toParticipant, isAsync: ctx.Options.Indication.UseAsync, "return");
        }

        return TryDeactivate(ctx, from);
    }

    internal static bool RenderDeactivateCallee<T>(RenderContext<T> ctx, string reason)
        where T : IUmlParticipant
    {
        var from = ctx.Step1;
        var to = ctx.Caller;
        if(ctx.Options.Indication.UseDone)
        {
            var fromParticipant = ctx.Diagram.AddParticipant(from);
            var toParticipant = ctx.Diagram.AddParticipant(to);

            ctx.Diagram.AddTransition(fromParticipant, toParticipant, isAsync: ctx.Options.Indication.UseAsync, reason);
        }
        return TryDeactivate(ctx, from);
    }
}