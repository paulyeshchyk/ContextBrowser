using ContextBrowser.UmlKit.Diagrams;
using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

internal static class TransitionRenderer
{
    public static void RenderTransition(UmlTransitionDto t, UmlDiagram diagram, ContextTransitionDiagramBuilderOptions builderOptions)
    {
        var callerPart = builderOptions.UseMethodAsParticipant ? t.CallerId : t.CallerClassName;
        var calleePart = builderOptions.UseMethodAsParticipant ? t.CalleeId : t.CalleeClassName;

        diagram.AddParticipant(callerPart, builderOptions.DefaultParticipantKeyword);
        diagram.AddParticipant(calleePart, builderOptions.DefaultParticipantKeyword);

        switch(builderOptions.DetailLevel)
        {
            case DiagramDetailLevel.Summary:
                RenderSummaryTransition(diagram, t);
                break;
            case DiagramDetailLevel.Method:
                RenderMethodTransition(diagram, t);
                break;
            case DiagramDetailLevel.Full:
                RenderFullTransition(diagram, t);
                break;
        }
    }

    private static void RenderSummaryTransition(UmlDiagram diagram, UmlTransitionDto t)
    {
        diagram.AddTransition(t.CallerClassName, t.CalleeClassName, "calls");
        //diagram.Activate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
        //diagram.Deactivate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
    }

    private static void RenderMethodTransition(UmlDiagram diagram, UmlTransitionDto t)
    {
        diagram.AddTransition(t.CallerClassName, t.CalleeClassName, t.CalleeMethod);
        //diagram.Activate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
        diagram.Deactivate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
    }

    private static void RenderFullTransition(UmlDiagram diagram, UmlTransitionDto t)
    {
        if(!string.IsNullOrWhiteSpace(t.RunContext))
        {
            diagram.AddParticipant(t.RunContext, UmlParticipantKeyword.Control);
            diagram.AddTransition(t.CallerClassName, t.RunContext, t.CallerMethod);
            //diagram.Activate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
            //diagram.Deactivate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
            diagram.AddTransition(t.RunContext, t.CalleeClassName, t.CalleeMethod);
            diagram.AddTransition(t.CalleeClassName, t.RunContext, "return");
            diagram.AddTransition(t.RunContext, t.CallerClassName, "done");
        }
        else
        {
            //diagram.Activate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
            diagram.AddTransition(t.CallerClassName, t.CalleeClassName, t.CalleeMethod);
            //diagram.Deactivate(new UmlDeclarableDto("empty declaration", t.CalleeClassName));
            diagram.AddTransition(t.CalleeClassName, t.CallerClassName, "done");
        }
    }
}
