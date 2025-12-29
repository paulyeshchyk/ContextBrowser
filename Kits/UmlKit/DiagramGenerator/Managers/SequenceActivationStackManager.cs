using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Builders.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Managers;

public static partial class SequenceActivationStackManager
{
    // Отвечает за управление стеком активации.
    internal static bool TryActivate<T>(RenderContext<T> ctx, string source, string? destination, string reason)
        where T : IUmlParticipant
    {
        if (!ctx.Options.Activation.UseActivation || string.IsNullOrWhiteSpace(destination))
        {
            return false;
        }

        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"Stack activate {destination}");

        if (ctx.Options.Activation.UseActivationCall)
        {
            ctx.Diagram.Activate(source: source, destination, reason: ctx.Options.Debug ? $"INVOCATION ACTIVATE [ {reason} ]" : $"{reason}", softActivation: false);
        }

        return TryPush(ctx, destination);
    }

    internal static bool TryDeactivate<T>(RenderContext<T> ctx, string? participant)
        where T : IUmlParticipant
    {
        if (!ctx.Options.Activation.UseActivation || string.IsNullOrWhiteSpace(participant))
        {
            return false;
        }

        if (ctx.ActivationStack.TryPeek(out var top) && top == participant)
        {
            return TryPop(ctx, participant);
        }

        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Err, $"Stack deactivate {participant} - incorrect or empty stack.");
        return false;
    }

    internal static bool TryPush<T>(RenderContext<T> ctx, string destination)
        where T : IUmlParticipant
    {
        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"TryPush {destination}");
        ctx.ActivationStack.Push(destination);
        return true;
    }

    internal static bool TryPop<T>(RenderContext<T> ctx, string participant)
        where T : IUmlParticipant
    {
        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"TryPop {participant}");
        var item = ctx.ActivationStack.Pop();
        ctx.Diagram.Deactivate(item);
        return true;
    }
}