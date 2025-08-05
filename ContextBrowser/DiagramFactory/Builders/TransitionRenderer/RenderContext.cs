using ContextBrowser.DiagramFactory.Model;
using LoggerKit;
using UmlKit.Diagrams;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionRenderer;

internal sealed class RenderContext
{
    public UmlTransitionDto T { get; }

    public UmlDiagram Diagram { get; }

    public ContextTransitionDiagramBuilderOptions Options { get; }

    public Stack<string> ActivationStack { get; }

    public OnWriteLog? Log { get; }

    public string? Caller => T.CallerClassName;

    public string? Callee => T.CalleeClassName;

    public string? RunContext => T.RunContext;

    public string? CallerMethod => T.CallerMethod;

    public string? CalleeMethod => T.CalleeMethod;

    public string? Step1 => !string.IsNullOrWhiteSpace(RunContext) ? RunContext : Callee;

    public string? Step2 => !string.IsNullOrWhiteSpace(RunContext) ? Callee : null;

    public RenderContext(
        UmlTransitionDto t,
        UmlDiagram diagram,
        ContextTransitionDiagramBuilderOptions options,
        Stack<string> activationStack,
        OnWriteLog? log)
    {
        T = t;
        Diagram = diagram;
        Options = options;
        ActivationStack = activationStack;
        Log = log;
    }
}

