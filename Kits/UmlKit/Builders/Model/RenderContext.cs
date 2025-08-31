using ContextBrowserKit.Log;
using ContextBrowserKit.Options;
using LoggerKit;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Builders.Model;

public sealed class RenderContext<T>
    where T : IUmlParticipant
{
    public UmlTransitionDto Transition { get; }

    public UmlDiagram<T> Diagram { get; }

    public DiagramBuilderOptions Options { get; }

    public RenderContextActivationStack ActivationStack { get; }

    public IAppLogger<AppLevel> Logger { get; }

    public string Caller => Transition.CallerClassName;

    public string Callee => Transition.CalleeClassName;

    public string RunContext => Transition.RunContext;

    public string? CallerMethod => Transition.CallerMethod;

    public string? CalleeMethod => Transition.CalleeMethod;

    public string RunContextOrCallee => !string.IsNullOrWhiteSpace(RunContext) ? RunContext : Callee;

    public RenderContext(UmlTransitionDto t, UmlDiagram<T> diagram, DiagramBuilderOptions options, RenderContextActivationStack activationStack, IAppLogger<AppLevel> logger)
    {
        Transition = t;
        Diagram = diagram;
        Options = options;
        ActivationStack = activationStack;
        Logger = logger;
    }
}
