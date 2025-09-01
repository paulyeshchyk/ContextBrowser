using System;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Builders.Model;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Managers;

public static partial class SequenceParticipantsManager
{
    internal static void AddParticipants<T>(RenderContext<T> ctx, UmlParticipantKeywordsSet defaultKeywords)
        where T : IUmlParticipant
    {
        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"Adding participants for context: {ctx.RunContext}", LogLevelNode.Start);

        var callerName = ctx.Transition.CallerClassName.AlphanumericOnly();
        var calleeName = ctx.Transition.CalleeClassName.AlphanumericOnly();

        var runContextName = ctx.RunContext?.AlphanumericOnly();

        // Стратегия по умолчанию: если RunContext пуст, используем уникальный, несовпадающий ID.
        var tempRunContextName = string.IsNullOrEmpty(runContextName) ? Guid.NewGuid().ToString() : runContextName;

        AddParticipantIfApplicable(callerName, tempRunContextName, ctx, defaultKeywords);

        // Добавляем Callee, только если он отличается от Caller, чтобы избежать дублирования
        if (!callerName.Equals(calleeName))
        {
            AddParticipantIfApplicable(calleeName, tempRunContextName, ctx, defaultKeywords);
        }

        // Если RunContext был задан, добавляем его с ключевым словом "Control".
        if (!string.IsNullOrEmpty(runContextName))
        {
            AddParticipant(ctx, defaultKeywords.Control, runContextName);
        }

        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private static void AddParticipantIfApplicable<T>(string className, string runContextName, RenderContext<T> ctx, UmlParticipantKeywordsSet defaultKeywords)
        where T : IUmlParticipant
    {
        var classAlpha = className.AlphanumericOnly();

        var keyword = classAlpha.Equals(runContextName) ? defaultKeywords.Control : defaultKeywords.Actor;

        if (!classAlpha.Equals(runContextName))
        {
            AddParticipant<T>(ctx, keyword, classAlpha);
        }
    }

    private static void AddParticipant<T>(RenderContext<T> ctx, UmlParticipantKeyword keyword, string p)
        where T : IUmlParticipant
    {
        ctx.Logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"Adding participant [{p}][{ctx.Transition.CallerClassName}.{ctx.Transition.CallerMethod} -> {ctx.Transition.CalleeClassName}.{ctx.Transition.CalleeMethod}]");
        ctx.Diagram.AddParticipant(p, keyword: keyword);
    }
}