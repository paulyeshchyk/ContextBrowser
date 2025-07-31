using ContextBrowser.ContextKit.Model;
using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextBrowser.extensions;
using ContextBrowser.LoggerKit;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly ContextTransitionDiagramBuilderOptions _options;
    private readonly List<ITransitionBuilder> _transitionBuilders;
    private readonly OnWriteLog? _onWriteLog = null;

    public ContextTransitionDiagramBuilder(ContextTransitionDiagramBuilderOptions? options, IEnumerable<ITransitionBuilder> transitionBuilders, OnWriteLog? onWriteLog = null)
    {
        _options = options ?? new ContextTransitionDiagramBuilderOptions(detailLevel: DiagramDetailLevel.Full, direction: DiagramDirection.BiDirectional, defaultParticipantKeyword: UmlKit.Model.UmlParticipantKeyword.Actor, useMethodAsParticipant: false);
        _transitionBuilders = transitionBuilders.ToList();
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

        if (!methods.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Warn, $"No methods for domain '{domainName}'");
            return false;
        }

        var allTransitions = new HashSet<UmlTransitionDto>();

        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, IndentedAppLoggerHelpers.SStartTag);
        foreach (var builder in _transitionBuilders)
        {
            var transitions = builder.BuildTransitions(methods, allContexts);
            _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, IndentedAppLoggerHelpers.SStartTag);

            foreach (var t in transitions)
            {
                _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, $"Writing transition: {t.CallerName} -> {t.CalleeName}");
                allTransitions.Add(t);
            }

            _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, IndentedAppLoggerHelpers.SEndTag);
        }
        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, IndentedAppLoggerHelpers.SEndTag);

        if (!allTransitions.Any())
            return false;

        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, IndentedAppLoggerHelpers.SStartTag);

        foreach (var t in allTransitions)
        {
            TransitionRenderer.RenderFullTransition(diagram, t, _options, _onWriteLog);
        }

        _onWriteLog?.Invoke(AppLevel.PumlTransition, LogLevel.Dbg, IndentedAppLoggerHelpers.SEndTag);


        TransitionRenderer.FinalizeDiagram(diagram);

        return true;
    }
}
