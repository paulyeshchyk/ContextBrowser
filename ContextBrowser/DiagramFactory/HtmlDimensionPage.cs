using ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;
using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextBrowser.ExporterKit.Puml;
using ContextBrowser.Infrastructure;
using ContextBrowser.Infrastructure.Compiler;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using LoggerKit;
using LoggerKit.Model;
using System.Text;

namespace ContextBrowser.DiagramFactory;

// context: html, build
internal class HtmlContextDimensionBuilder
{
    private readonly Dictionary<ContextContainer, List<string>> _matrix;
    private readonly List<ContextInfo> _allContexts;
    private readonly string _outputDirectory;
    private readonly AppOptions _options;
    private readonly TransitionRenderer _renderer;
    private readonly OnWriteLog? _onWriteLog;
    private Dictionary<string, bool> _renderedSequenceDomains = new Dictionary<string, bool>();
    private Dictionary<string, bool> _renderedStateDomains = new Dictionary<string, bool>();


    public HtmlContextDimensionBuilder(Dictionary<ContextContainer, List<string>> matrix, List<ContextInfo> allContexts, string outputDirectory, AppOptions options, OnWriteLog? onWriteLog)
    {
        _matrix = matrix;
        _allContexts = allContexts;
        _outputDirectory = outputDirectory;
        _options = options;
        _onWriteLog = onWriteLog;
        _renderer = new TransitionRenderer(onWriteLog);
    }

    // context: html, build
    public void Build()
    {
        GenerateDomainDiagrams();
        GenerateHtmlPages();
    }

    // context: html, build
    internal void GenerateDomainDiagrams()
    {
        var classifier = new ContextClassifier();

        foreach(var domain in _matrix.Select(k => k.Key.Domain).Distinct())
        {
            _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, $"Compiling Domain [{domain}]", LogLevelNode.Start);

            var bf = ContextDiagramFactory.Transition(_options, _onWriteLog);
            var sequenceDiagramCompiler = new SequenceDiagramCompiler(classifier, _outputDirectory, _onWriteLog, _options, bf);
            var rendered_sequence = sequenceDiagramCompiler.Compile(domain, _allContexts);
            _renderedSequenceDomains[domain] = rendered_sequence;

            var bf2 = ContextDiagramFactory.Custom(_options.diagramType, _options, _onWriteLog);
            var stateDiagramCompiler = new StateDiagramCompiler(classifier, _options, bf2, _outputDirectory, _renderer, _onWriteLog);
            var rendered_state = stateDiagramCompiler.Compile(domain, _allContexts);
            _renderedStateDomains[domain] = rendered_state;

            _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        }
    }

    // context: html, build
    internal void GenerateHtmlPages()
    {
        var allActions = _matrix.Keys.Select(k => k.Action).Distinct();
        var allDomains = _matrix.Keys.Select(k => k.Domain).Distinct();

        // --- ACTIONS ---
        foreach(var action in allActions)
        {
            var methods = _matrix
                .Where(kvp => kvp.Key.Action == action)
                .SelectMany(kvp => kvp.Value)
                .Distinct()
                .ToList();

            string embeddedScript = string.Empty;
            string embeddedContent = string.Empty;

            if(_renderedStateDomains.TryGetValue(action, out var rendered_sequence) && rendered_sequence == true)
            {
                var puml = PumlInjector.InjectDomainEmbeddedHtml(action, _outputDirectory);
                embeddedScript = puml.EmbeddedScript;
                embeddedContent = puml.EmbeddedContent;
            }

            var path = Path.Combine(_outputDirectory, $"action_{action}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer,() =>
            {
                HtmlBuilderFactory.Head.With(writer,() =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Action: {action}");
                });

                HtmlBuilderFactory.Body.With(writer,() =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"Action: {action}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer,() =>
                    {
                        foreach(var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method);
                    });

                    if(!string.IsNullOrWhiteSpace(embeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                });
            });
        }

        // --- DOMAINS ---
        foreach(var domain in allDomains)
        {
            var methods = _matrix
                .Where(kvp => kvp.Key.Domain == domain)
                .SelectMany(kvp => kvp.Value)
                .Distinct()
                .ToList();

            string embeddedScript = string.Empty;
            string embeddedContent = string.Empty;

            if(_renderedSequenceDomains.TryGetValue(domain, out var rendered_sequence) && rendered_sequence == true)
            {
                var puml = PumlInjector.InjectDomainEmbeddedHtml(domain, _outputDirectory);
                embeddedScript = puml.EmbeddedScript;
                embeddedContent = puml.EmbeddedContent;
            }

            var path = Path.Combine(_outputDirectory, $"sequence_domain_{domain}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer,() =>
            {
                HtmlBuilderFactory.Head.With(writer,() =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Domain: {domain}");

                    if(!string.IsNullOrWhiteSpace(embeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedScript);
                });

                HtmlBuilderFactory.Body.With(writer,() =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"Domain: {domain}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer,() =>
                    {
                        foreach(var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method);
                    });

                    if(!string.IsNullOrWhiteSpace(embeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                });
            });
        }
    }
}