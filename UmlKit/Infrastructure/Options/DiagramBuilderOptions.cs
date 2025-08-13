namespace UmlKit.Infrastructure.Options;

public record DiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel { get; set; }

    public DiagramDirection Direction { get; set; }

    public bool UseActivation { get; set; }

    public bool UseSelfCallContinuation { get; set; }

    public bool UseReturn { get; set; }

    public bool UseAsync { get; set; }

    public TreeBuilderMode Mode { get; set; }

    public DiagramBuilderKeys DiagramType { get; set; }

    public DiagramBuilderOptions(
        DiagramDetailLevel detailLevel,
        DiagramDirection direction,
        bool useActivation,
        bool useReturn,
        bool useAsync,
        bool useSelfCallContinuation,
        TreeBuilderMode useContextTransitionTreeBuilderMode,
        DiagramBuilderKeys diagramType)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        UseActivation = useActivation;
        UseReturn = useReturn;
        UseAsync = useAsync;
        UseSelfCallContinuation = useSelfCallContinuation;
        Mode = useContextTransitionTreeBuilderMode;
        DiagramType = diagramType;
    }

    public enum TreeBuilderMode
    {
        FromParentToChild,
        FromChildToParent,
        BiDirectional
    }
}
