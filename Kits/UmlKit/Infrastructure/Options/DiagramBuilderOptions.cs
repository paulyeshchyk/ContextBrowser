using UmlKit.Infrastructure.Options.Activation;
using UmlKit.Infrastructure.Options.Indication;

namespace UmlKit.Infrastructure.Options;

// parsing: error
public partial record DiagramBuilderOptions
{
    public bool Debug { get; set; }

    public DiagramDetailLevel DetailLevel { get; set; }

    public DiagramDirection Direction { get; set; }

    public DiagramActivationOptions Activation { get; set; }

    public DiagramTransitionOptions CalleeTransitionOptions { get; set; }

    public DiagramInvocationOption InvocationOptions { get; set; }

    public DiagramIndicationOption Indication { get; set; }

    public DiagramBuilderTreeMode TreeMode { get; set; }

    public DiagramBuilderKeys DiagramType { get; set; }

    public DiagramBuilderOptions(
        bool debug,
        DiagramDetailLevel detailLevel,
        DiagramDirection direction,
        DiagramActivationOptions activation,
        DiagramTransitionOptions transitionOptions,
        DiagramInvocationOption invocationOption,
        DiagramIndicationOption indication,
        DiagramBuilderTreeMode treeMode,
        DiagramBuilderKeys diagramType)
    {
        DetailLevel = detailLevel;
        Direction = direction;
        Activation = activation;
        CalleeTransitionOptions = transitionOptions;
        InvocationOptions = invocationOption;
        Indication = indication;
        TreeMode = treeMode;
        DiagramType = diagramType;
        Debug = debug;
    }
}
