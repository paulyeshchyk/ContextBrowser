using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

public record ContextTransitionDiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel = DiagramDetailLevel.Method;

    public DiagramDirection Direction = DiagramDirection.BiDirectional;

    public UmlParticipantKeyword DefaultParticipantKeyword = UmlParticipantKeyword.Actor;

    public bool UseMethodAsParticipant = false;
}
