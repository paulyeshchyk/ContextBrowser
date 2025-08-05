using ContextBrowser.DiagramFactory.Model;
using LoggerKit;
using UmlKit.Diagrams;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionRenderer;

internal sealed class RenderContext
{
    public UmlTransitionDto Transition { get; }

    public UmlDiagram Diagram { get; }

    public ContextTransitionDiagramBuilderOptions Options { get; }

    public Stack<string> ActivationStack { get; }

    public OnWriteLog? Log { get; }

    public string? Caller => Transition.CallerClassName;

    public string? Callee => Transition.CalleeClassName;

    public string? RunContext => Transition.RunContext;

    public string? CallerMethod => Transition.CallerMethod;

    public string? CalleeMethod => Transition.CalleeMethod;

    public string? Step1 => !string.IsNullOrWhiteSpace(RunContext) ? RunContext : Callee;

    public string? Step2 => !string.IsNullOrWhiteSpace(RunContext) ? Callee : null;

    public RenderContext(
        UmlTransitionDto t,
        UmlDiagram diagram,
        ContextTransitionDiagramBuilderOptions options,
        Stack<string> activationStack,
        OnWriteLog? log)
    {
        Transition = t;
        Diagram = diagram;
        Options = options;
        ActivationStack = activationStack;
        Log = log;
    }
}

