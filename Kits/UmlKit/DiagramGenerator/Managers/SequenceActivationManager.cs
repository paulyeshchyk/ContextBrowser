using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Builders.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Managers;

public static partial class SequenceActivationManager
{
    // Отвечает за активацию и деактивацию участников.
    internal static bool RenderActivateCaller<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        return SequenceActivationStackManager.TryActivate(
            ctx,
            source: ctx.Caller,
            destination: ctx.Caller,
            reason: ctx.Options.Debug ? $"CALLER ACTIVATE [ {ctx.CalleeMethod} ]" : $"{ctx.CalleeMethod}");
    }

    internal static bool RenderDeactivateCaller<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        return SequenceActivationStackManager.TryDeactivate(ctx, ctx.Caller);
    }

    internal static bool RenderActivateCallee<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        var from = ctx.Caller.AlphanumericOnly();
        var to = ctx.RunContextOrCallee.AlphanumericOnly();
        var label = ctx.CalleeMethod;

        if (!ctx.Options.CalleeTransitionOptions.UseCall)
        {
            return true;
        }

        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"Render CallerToStep1 transition [{from} -> {to}]:<{label}>");

        var reason = ctx.Options.Debug ? $"CALLER CALLEE ACTIVATE [ {ctx.CalleeMethod} ]" : $"{ctx.CalleeMethod}";

        if (!ctx.Options.Activation.UseActivation)
        {
            SequenceTransitionManager.TryAddTransition(from, to, ctx.Diagram, ctx.Options.Indication, reason);
            return true;
        }

        return SequenceActivationStackManager.TryActivate(ctx, source: from, destination: to, reason);
    }

    internal static bool RenderDeactivateCallee<T>(RenderContext<T> ctx)
        where T : IUmlParticipant
    {
        var from = ctx.RunContextOrCallee;
        var to = ctx.Caller;

        if (ctx.Options.CalleeTransitionOptions.UseDone)
        {
            SequenceTransitionManager.TryAddTransition(from, to, ctx.Diagram, ctx.Options.Indication, "done");
        }

        return SequenceActivationStackManager.TryDeactivate(ctx, from);
    }
}
