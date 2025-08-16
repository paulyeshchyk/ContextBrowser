namespace UmlKit.Infrastructure.Options;

// parsing: error
public record DiagramBuilderOptions
{
    public bool Debug { get; set; }

    public DiagramDetailLevel DetailLevel { get; set; }

    public DiagramDirection Direction { get; set; }

    public bool UseActivation { get; set; }

    public DiagramBuilderIndication Indication { get; set; }

    public TreeBuilderMode Mode { get; set; }

    public DiagramBuilderKeys DiagramType { get; set; }

    public DiagramBuilderOptions(
        bool debug,
        DiagramDetailLevel detailLevel,
        DiagramDirection direction,
        bool useActivation,
        DiagramBuilderIndication indication,
        TreeBuilderMode useContextTransitionTreeBuilderMode,
        DiagramBuilderKeys diagramType)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        UseActivation = useActivation;
        Indication = indication;
        Mode = useContextTransitionTreeBuilderMode;
        DiagramType = diagramType;
        Debug = debug;
    }

    public enum TreeBuilderMode
    {
        FromParentToChild,
        FromChildToParent,
        BiDirectional
    }
}

public record DiagramBuilderIndication
{
    public bool UseSelfCallContinuation { get; set; }

    public bool UseCalleeInvocation { get; set; }

    public bool UseReturn { get; set; }

    public bool UseDone { get; set; }

    public bool UseAsync { get; set; }

    public bool UseCalleeActivation { get; set; }

    public bool PushAnnotation { get; set; }

    public DiagramBuilderIndication(bool useReturn, bool useDone, bool useAsync, bool useSelfCallContinuation, bool useCalleeInvocation, bool useCalleeActivation, bool pushAnnotation)
    {
        UseSelfCallContinuation = useSelfCallContinuation;
        UseCalleeInvocation = useCalleeInvocation;
        UseReturn = useReturn;
        UseDone = useDone;
        UseAsync = useAsync;
        UseCalleeActivation = useCalleeActivation;
        PushAnnotation = pushAnnotation;
    }
}