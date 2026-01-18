using System;
using System.Text.Json.Serialization;
using UmlKit.Infrastructure.Options.Activation;
using UmlKit.Infrastructure.Options.Indication;

namespace UmlKit.Infrastructure.Options;

// parsing: error
public partial record DiagramBuilderOptions
{
    public bool Debug { get; set; }

    public DiagramDetailLevel DiagramDetailLevel { get; set; }

    public DiagramDirection DiagramDirection { get; set; }

    public DiagramActivationOptions Activation { get; set; }

    public DiagramTransitionOptions CalleeTransitionOptions { get; set; }

    public DiagramInvocationOption InvocationOptions { get; set; }

    public DiagramIndicationOption Indication { get; set; }

    public DiagramBuilderKeys DiagramType { get; set; }

    [JsonConstructor]
    public DiagramBuilderOptions(
        bool debug,
        DiagramDetailLevel diagramDetailLevel,
        DiagramDirection diagramDirection,
        DiagramBuilderKeys diagramType,
        DiagramActivationOptions activation,
        DiagramTransitionOptions calleeTransitionOptions,
        DiagramInvocationOption invocationOptions,
        DiagramIndicationOption indication)
    {
        DiagramDetailLevel = diagramDetailLevel;
        DiagramDirection = diagramDirection;
        DiagramType = diagramType;
        Activation = activation;
        CalleeTransitionOptions = calleeTransitionOptions;
        InvocationOptions = invocationOptions;
        Indication = indication;
        Debug = debug;
    }
}
