using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Builders.Model;
using UmlKit.Model;

namespace UmlKit.DiagramGenerator.Managers;

public static partial class SequenceInvocationManager
{
    // Отвечает за логику вызовов и возвратов.
    internal static bool RenderCalleeInvocation<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        var activated = ActivateInvocation(
            ctx,
            from: ctx.RunContextOrCallee,
            to: ctx.Callee,
            reason: ctx.CalleeMethod,
            canActivate: ctx.Options.InvocationOptions.UseInvocation);

        if (!activated)
        {
            return false;
        }

        return DeactivateInvocation(
            ctx,
            from: ctx.Callee,
            to: ctx.RunContextOrCallee,
            rReason: "return",
            canAddTransition: ctx.Options.InvocationOptions.UseReturn);
    }

    internal static bool ActivateInvocation<T>(RenderContext<T> ctx, string from, string to, string? reason, bool canActivate)
        where T : IUmlParticipant
    {
        if (string.IsNullOrWhiteSpace(to) || !canActivate)
        {
            return false;
        }

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render transition S1S2 [{from} -> {to}]:<{reason}>");
        var dReason = ctx.Options.Debug ? $"CALLEE INVOCATION ACTIVATE [ {reason} ]" : $"{reason}";

        return SequenceActivationStackManager.TryActivate(ctx, source: from, destination: to, dReason);
    }

    internal static bool DeactivateInvocation<T>(RenderContext<T> ctx, string from, string to, string rReason, bool canAddTransition)
        where T : IUmlParticipant
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return false;
        }

        if (canAddTransition)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Render transition S2S1 [{from} -> {to}]:<return>");
            var diagram = ctx.Diagram;
            var opt = ctx.Options.Indication;
            SequenceTransitionManager.TryAddTransition(from, to, diagram, opt, rReason);
        }

        return SequenceActivationStackManager.TryDeactivate(ctx, from);
    }
}
