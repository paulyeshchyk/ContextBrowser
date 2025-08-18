using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Builders.Model;
using UmlKit.Model;

namespace UmlKit.DiagramGenerator.Managers;

public static partial class SequenceParticipantsManager
{
    // Отвечает только за добавление участников на диаграмму.
    internal static void AddParticipants<T>(RenderContext<T> ctx, UmlParticipantKeyword defaultParticipantKeyword)
        where T : IUmlParticipant
    {
        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Adding participants for context: {ctx.RunContext}", LogLevelNode.Start);

        var participants = new[]
        {
            ctx.Transition.CallerClassName,
            ctx.Transition.CalleeClassName,
            ctx.Transition.RunContext
        }.Where(x => !string.IsNullOrWhiteSpace(x)).Select(s => s!);

        foreach(var p in participants)
        {
            ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"Adding participant [{p}][{ctx.Transition.CallerClassName}.{ctx.Transition.CallerMethod} -> {ctx.Transition.CalleeClassName}.{ctx.Transition.CalleeMethod}]");
            ctx.Diagram.AddParticipant(p, keyword: defaultParticipantKeyword);
        }

        ctx.Log?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
