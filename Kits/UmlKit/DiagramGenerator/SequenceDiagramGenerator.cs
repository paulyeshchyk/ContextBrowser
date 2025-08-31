using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator;

public class SequenceDiagramGenerator<P>
        where P : IUmlParticipant
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly DiagramBuilderOptions _options;
    private readonly IUmlTransitionFactory<P> _factory;
    private readonly ISequenceDiagramRenderer<P> _renderer;

    public SequenceDiagramGenerator(ISequenceDiagramRenderer<P> renderer, DiagramBuilderOptions options, IAppLogger<AppLevel> logger, IUmlTransitionFactory<P> factory)
    {
        _options = options;
        _logger = logger;
        _factory = factory;
        _renderer = renderer;
    }

    public bool Generate(UmlDiagram<P> diagram, GrouppedSortedTransitionList? allTransitions, string domain)
    {
        if (allTransitions == null || !allTransitions.HasTransitions())
        {
            _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Warn, $"No transitions provided for [{domain}]");
            return false;
        }

        _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Dbg, $"Rendering Diagram transitions for [{domain}]", LogLevelNode.Start);

        _renderer.Render(diagram, allTransitions);

        _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return true;
    }
}

internal class LogObjectRenderingTransioDiagram : LogObject
{
    public LogObjectRenderingTransioDiagram(LogLevelNode logLevelNode = LogLevelNode.None) : base(LogLevel.Dbg, default, logLevelNode)
    {
    }
}