using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

public record ContextTransitionDiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel;

    public DiagramDirection Direction;

    public UmlParticipantKeyword DefaultParticipantKeyword;

    public bool UseMethodAsParticipant;

    public ContextTransitionDiagramBuilderOptions(DiagramDetailLevel detailLevel, DiagramDirection direction, UmlParticipantKeyword defaultParticipantKeyword, bool useMethodAsParticipant)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        DefaultParticipantKeyword = defaultParticipantKeyword;
        UseMethodAsParticipant = useMethodAsParticipant;
    }
}
