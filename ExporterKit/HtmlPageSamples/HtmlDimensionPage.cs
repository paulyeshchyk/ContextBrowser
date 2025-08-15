using ContextBrowser.Infrastructure.Compiler;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using System.Text;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.Renderer;

namespace ExporterKit.HtmlPageSamples;

// context: html, build
public class HtmlContextDimensionBuilder
{
    private readonly Dictionary<ContextContainer, List<string>> _matrix;
    private readonly List<ContextInfo> _allContexts;
    private readonly string _outputDirectory;
    private readonly DiagramBuilderOptions _options;
    private readonly OnWriteLog? _onWriteLog;
    private Dictionary<string, bool> _renderedSequenceDomains = new Dictionary<string, bool>();
    private Dictionary<string, bool> _renderedStateDomains = new Dictionary<string, bool>();
    private IContextClassifier _contextClassifier;

    public HtmlContextDimensionBuilder(Dictionary<ContextContainer, List<string>> matrix, List<ContextInfo> allContexts, IContextClassifier contextClassifier, string outputDirectory, DiagramBuilderOptions options, OnWriteLog? onWriteLog)
    {
        _matrix = matrix;
        _allContexts = allContexts;
        _outputDirectory = outputDirectory;
        _options = options;
        _onWriteLog = onWriteLog;
        _contextClassifier = contextClassifier;
    }

    // context: html, build
    public void BuildDimension()
    {
        GenerateDomainDiagrams();
        GenerateHtmlPages();
    }

    internal void GenerateDomainDiagrams()
    {
        foreach(var domain in _matrix.Select(k => k.Key.Domain).Distinct())
        {
            CompileDomain(_contextClassifier, domain);
        }
    }

    internal void CompileDomain(IContextClassifier classifier, string domain)
    {
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Domain [{domain}]", LogLevelNode.Start);

        var bf = ContextDiagramFactory.Transition(_options, _onWriteLog);
        var diagramCompilerSequence = new SequenceDiagramCompiler(classifier, _outputDirectory, _onWriteLog, _options, bf);
        var rendered_sequence = diagramCompilerSequence.Compile(domain, _allContexts);
        _renderedSequenceDomains[domain] = rendered_sequence;

        var _factory = new UmlTransitionStateFactory();
        var _renderer = new SequenceRenderer<UmlState>(_options, _onWriteLog, _factory);
        var bf2 = ContextDiagramFactory.Custom(_options.DiagramType, _options, _onWriteLog);
        var diagramCompilerState = new StateDiagramCompiler(classifier, _options, bf2, _outputDirectory, _renderer, _onWriteLog);
        var rendered_state = diagramCompilerState.Compile(domain, _allContexts);
        _renderedStateDomains[domain] = rendered_state;

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }

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