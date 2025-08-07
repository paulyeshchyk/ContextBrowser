using ContextBrowser.DiagramFactory.Renderer.Model;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Diagrams;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Renderer;

internal sealed class RenderContext
{
    public UmlTransitionDto Transition { get; }

    public UmlDiagram Diagram { get; }

    public ContextTransitionDiagramBuilderOptions Options { get; }

    public ActivationStack ActivationStack { get; }

    public OnWriteLog? Log { get; }

    public string Caller => Transition.CallerClassName;

    public string Callee => Transition.CalleeClassName;

    public string? RunContext => Transition.RunContext;

    public string? CallerMethod => Transition.CallerMethod;

    public string? CalleeMethod => Transition.CalleeMethod;

    public string? Step1 => !string.IsNullOrWhiteSpace(RunContext) ? RunContext : Callee;

    public string? Step2 => !string.IsNullOrWhiteSpace(RunContext) ? Callee : null;

    public RenderContext(
        UmlTransitionDto t,
        UmlDiagram diagram,
        ContextTransitionDiagramBuilderOptions options,
        ActivationStack activationStack,
        OnWriteLog? log)
    {
        Transition = t;
        Diagram = diagram;
        Options = options;
        ActivationStack = activationStack;
        Log = log;
    }
}

public class ActivationStack : Stack<string>
{
    private readonly OnWriteLog? _onWriteLog;
    public ActivationStack(OnWriteLog? onWriteLog) : base()
    {
        _onWriteLog = onWriteLog;
    }

    public void TryPush(string? value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, "[PUSH_FAIL]: item is empty");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[PUSH_OK]: {value}");
        Push(value);
    }

    public bool TryPeek2(out String? result)
    {
        var isSuccess = TryPeek(out result);
        if(isSuccess)
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[PEEK_OK]: {result}");
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"[PEEK_FAIL] Can't peek");
        }
        return isSuccess;
    }
}