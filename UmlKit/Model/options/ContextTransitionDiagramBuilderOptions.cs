namespace UmlKit.Model.Options;

public record ContextTransitionDiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel;

    public DiagramDirection Direction;

    public UmlParticipantKeyword DefaultParticipantKeyword;

    public bool UseMethodAsParticipant;

    public bool UseActivation;

    public ContextTransitionDiagramBuilderOptions(DiagramDetailLevel detailLevel, DiagramDirection direction, UmlParticipantKeyword defaultParticipantKeyword, bool useMethodAsParticipant, bool useActivation)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        DefaultParticipantKeyword = defaultParticipantKeyword;
        UseMethodAsParticipant = useMethodAsParticipant;
        UseActivation = useActivation;
    }
}