using ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders.Model;
using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextBrowser.DiagramFactory.Model;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Diagrams;


namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly ContextTransitionDiagramBuilderOptions _options;
    private readonly List<ITransitionBuilder> _transitionBuilders;
    private readonly OnWriteLog? _onWriteLog = null;

    public ContextTransitionDiagramBuilder(ContextTransitionDiagramBuilderOptions options, IEnumerable<ITransitionBuilder> transitionBuilders, OnWriteLog? onWriteLog = null)
    {
        _options = options;
        _transitionBuilders = transitionBuilders.Where(b => b.Direction == options.Direction).ToList();
        _onWriteLog = onWriteLog;
    }

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram diagram)
    {
        var methods = allContexts
            .Where(ctx =>
                ctx.ElementType == ContextInfoElementType.method &&
                ctx.Domains.Contains(domainName) &&
                classifier.HasActionAndDomain(ctx))
            .ToList();

        if(!methods.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Err, $"No methods found for domain '{domainName}'");
            return false;
        }

        HashSet<UmlTransitionDto> allTransitions = BuildAllTransitions(domainName, allContexts, methods);

        if(!allTransitions.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Err, $"No transitions found for domain '{domainName}'");
            return false;
        }

        RenderAllTransitions(diagram, allTransitions);

        return true;
    }

    private void RenderAllTransitions(UmlDiagram diagram, HashSet<UmlTransitionDto> allTransitions)
    {
        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, "RenderAllTransitions", LogLevelNode.Start);

        foreach(var t in allTransitions)
        {
            TransitionRenderer.RenderFullTransition(diagram, t, _options, _onWriteLog);
        }

        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        TransitionRenderer.FinalizeDiagram(diagram);
    }

    private HashSet<UmlTransitionDto> BuildAllTransitions(string domainName, List<ContextInfo> allContexts, List<ContextInfo> methods)
    {
        var allTransitions = new HashSet<UmlTransitionDto>();

        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, $"Build transitions for domain: {domainName}", LogLevelNode.Start);
        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, $"Build transitions for domain: {domainName}");
        if(!_transitionBuilders.Any())
        {
            _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Err, $"No transition builders provided for domain: {domainName}");
            return allTransitions;
        }

        foreach(var builder in _transitionBuilders)
        {
            var transitions = builder.BuildTransitions(methods, allContexts);
            _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, $"Build Transition {builder.GetType().Name}", LogLevelNode.Start);

            foreach(var t in transitions)
            {
                _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, $"Writing transition: {t.CallerClassName}.{t.CallerMethod} -> {t.CalleeClassName}.{t.CalleeMethod}");
                allTransitions.Add(t);
            }

            _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        }
        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return allTransitions;
    }
}