namespace UmlKit.Model.Options;

public record ContextTransitionDiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel;

    public DiagramDirection Direction;

    public bool UseMethodAsParticipant;

    public bool UseActivation;

    public bool UseSelfCallContinuation;

    public bool UseReturn;

    public ContextTransitionTreeBuilderMode TreeBuilderMode;

    public bool CollapseCalleeClassIfSameAsCaller;

    public ContextTransitionDiagramBuilderOptions(DiagramDetailLevel detailLevel, DiagramDirection direction, bool useMethodAsParticipant, bool useActivation, bool useReturn, bool useSelfCallContinuation, ContextTransitionTreeBuilderMode useContextTransitionTreeBuilderMode, bool collapseCalleeClassIfSameAsCaller)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        UseMethodAsParticipant = useMethodAsParticipant;
        UseActivation = useActivation;
        UseReturn = useReturn;
        UseSelfCallContinuation = useSelfCallContinuation;
        TreeBuilderMode = useContextTransitionTreeBuilderMode;
        CollapseCalleeClassIfSameAsCaller = collapseCalleeClassIfSameAsCaller;
    }
}

public enum ContextTransitionTreeBuilderMode
{
    FromParentToChild,
    FromChildToParent,
    BiDirectional
}