namespace UmlKit.Model.Options;

public record ContextTransitionDiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel;

    public DiagramDirection Direction;

    public bool UseActivation;

    public bool UseSelfCallContinuation;

    public bool UseReturn;

    public bool UseAsync;

    public ContextTransitionTreeBuilderMode TreeBuilderMode;


    public ContextTransitionDiagramBuilderOptions(DiagramDetailLevel detailLevel, DiagramDirection direction, bool useActivation, bool useReturn, bool useAsync, bool useSelfCallContinuation, ContextTransitionTreeBuilderMode useContextTransitionTreeBuilderMode)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        UseActivation = useActivation;
        UseReturn = useReturn;
        UseAsync = useAsync;
        UseSelfCallContinuation = useSelfCallContinuation;
        TreeBuilderMode = useContextTransitionTreeBuilderMode;
    }
}

public enum ContextTransitionTreeBuilderMode
{
    FromParentToChild,
    FromChildToParent,
    BiDirectional
}