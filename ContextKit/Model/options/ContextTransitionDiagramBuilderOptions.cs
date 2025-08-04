namespace UmlKit.Model.Options;

public record ContextTransitionDiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel;

    public DiagramDirection Direction;

    public bool UseMethodAsParticipant;

    public bool UseActivation;

    public bool UseSelfCallContinuation;

    public ContextTransitionDiagramBuilderOptions(DiagramDetailLevel detailLevel, DiagramDirection direction, bool useMethodAsParticipant, bool useActivation, bool useSelfCallContinuation)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        UseMethodAsParticipant = useMethodAsParticipant;
        UseActivation = useActivation;
        UseSelfCallContinuation = useSelfCallContinuation;
    }
}