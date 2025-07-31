using ContextBrowser.extensions;
using ContextBrowser.LoggerKit;
using ContextBrowser.UmlKit.Diagrams;
using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

internal static class TransitionRenderer
{
    private static readonly HashSet<string> activated = new();

    public static void RenderFullTransition(UmlDiagram diagram, UmlTransitionDto t, ContextTransitionDiagramBuilderOptions _options, OnWriteLog? onWriteLog = null)
    {
        // Активируем вызывающего, если ещё не активирован
        if (!string.IsNullOrWhiteSpace(t.CallerClassName) && activated.Add(t.CallerClassName))
        {
            diagram.Activate(t.CallerClassName);
            onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, $"Активируем {t.CallerClassName}");
        }
        else
        {
            onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, $"Не активируем {t.CallerClassName}");
        }

        // Контекст исполнения (если RunContext есть)
        if (!string.IsNullOrWhiteSpace(t.RunContext))
        {
            diagram.AddParticipant(t.RunContext, UmlParticipantKeyword.Control);

            // Активируем контрол, если он ещё не активирован
            if (activated.Add(t.RunContext))
            {
                diagram.Activate(t.RunContext);
            }

            diagram.AddTransition(t.CallerClassName, t.RunContext, t.CallerMethod);
            diagram.AddTransition(t.RunContext, t.CalleeClassName, t.CalleeMethod);
            diagram.AddTransition(t.CalleeClassName, t.RunContext, "return");
            diagram.AddTransition(t.RunContext, t.CallerClassName, "done");
        }
        else
        {
            // Активируем вызываемого, если ещё не активирован
            if (!string.IsNullOrWhiteSpace(t.CalleeClassName) && activated.Add(t.CalleeClassName))
            {
                diagram.Activate(t.CalleeClassName);
            }

            diagram.AddTransition(t.CallerClassName, t.CalleeClassName, t.CalleeMethod);
            diagram.AddTransition(t.CalleeClassName, t.CallerClassName, "done");
        }
    }

    public static void FinalizeDiagram(UmlDiagram diagram)
    {
        foreach (var participant in activated)
        {
            diagram.Deactivate(participant);
        }

        activated.Clear(); // на случай повторных генераций
    }
}