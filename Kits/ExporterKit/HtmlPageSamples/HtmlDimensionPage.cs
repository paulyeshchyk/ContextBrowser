using ContextBrowser.Infrastructure.Compiler;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using System.Text;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace ExporterKit.HtmlPageSamples;

// context: html, build
public class HtmlContextDimensionBuilder
{
    private readonly IContextInfoMatrix _matrix;
    private readonly List<ContextInfo> _allContexts;
    private readonly ExportOptions _exportOptions;
    private readonly DiagramBuilderOptions _options;
    private readonly OnWriteLog? _onWriteLog;
    private Dictionary<string, bool> _renderedSequenceDomains = new Dictionary<string, bool>();
    private Dictionary<string, bool> _renderedStateDomains = new Dictionary<string, bool>();
    private IContextClassifier _contextClassifier;

    public HtmlContextDimensionBuilder(IContextInfoMatrix matrix, List<ContextInfo> allContexts, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, OnWriteLog? onWriteLog)
    {
        _matrix = matrix;
        _allContexts = allContexts;
        _exportOptions = exportOptions;
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

    // context: html, build
    internal void GenerateDomainDiagrams()
    {
        var domains = _matrix.GetDomains();
        foreach (var domain in domains.Distinct())
        {
            CompileDomain(_contextClassifier, domain);
        }
    }

    internal void CompileDomain(IContextClassifier classifier, string domain)
    {
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Domain [{domain}]", LogLevelNode.Start);

        var bf = ContextDiagramFactory.Transition(_options, _onWriteLog);
        var diagramCompilerSequence = new SequenceDiagramCompiler(classifier, _exportOptions, _onWriteLog, _options, bf);
        var rendered_sequence = diagramCompilerSequence.Compile(domain, _allContexts);
        _renderedSequenceDomains[domain] = rendered_sequence;

        var _factory = new UmlTransitionStateFactory();
        var renderer = new SequenceDiagramRendererPlain<UmlState>(_onWriteLog, _options, _factory);
        var _generator = new SequenceDiagramGenerator<UmlState>(renderer, _options, _onWriteLog, _factory);
        var bf2 = ContextDiagramFactory.Custom(_options.DiagramType, _options, _onWriteLog);
        var diagramCompilerState = new StateDiagramCompiler(classifier, _options, bf2, _exportOptions, _generator, _onWriteLog);
        var rendered_state = diagramCompilerState.Compile(domain, _allContexts);
        _renderedStateDomains[domain] = rendered_state;

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }

    internal void GenerateHtmlPages()
    {
        var allActions = _matrix.GetActions().Distinct();
        var allDomains = _matrix.GetDomains().Distinct();

        // --- ACTIONS ---
        foreach (var action in allActions)
        {
            var methods = _matrix.GetMethodsByAction(action: action).ToList();

            string embeddedScript = string.Empty;
            string embeddedContent = string.Empty;

            if (_renderedStateDomains.TryGetValue(action, out var rendered_sequence) && rendered_sequence == true)
            {
                var puml = PumlInjector.InjectDomainEmbeddedHtml(action, _exportOptions);
                embeddedScript = puml.EmbeddedScript;
                embeddedContent = puml.EmbeddedContent;
            }

            var path = ExportPathBuilder.BuildPath(_exportOptions.Paths, ExportPathType.pages, $"action_{action}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer, () =>
            {
                HtmlBuilderFactory.Head.With(writer, () =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Action: {action}");
                });

                HtmlBuilderFactory.Body.With(writer, () =>
                {
                    HtmlBuilderFactory.Paragraph.Cell(writer, "index", "..\\index.html");
                    HtmlBuilderFactory.H1.Cell(writer, $"Action: {action}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method.FullName);
                    });

                    if (!string.IsNullOrWhiteSpace(embeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                });
            });
        }

        // --- DOMAINS ---
        foreach (var domain in allDomains)
        {
            var methods = _matrix.GetMethodsByDomain(domain);

            string embeddedScript = string.Empty;
            string embeddedContent = string.Empty;

            if (_renderedSequenceDomains.TryGetValue(domain, out var rendered_sequence) && rendered_sequence == true)
            {
                var puml = PumlInjector.InjectDomainEmbeddedHtml(domain, _exportOptions);
                embeddedScript = puml.EmbeddedScript;
                embeddedContent = puml.EmbeddedContent;
            }

            var path = ExportPathBuilder.BuildPath(_exportOptions.Paths, ExportPathType.pages, $"sequence_domain_{domain}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer, () =>
            {
                HtmlBuilderFactory.Head.With(writer, () =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Domain: {domain}");

                    if (!string.IsNullOrWhiteSpace(embeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedScript);
                });

                HtmlBuilderFactory.Body.With(writer, () =>
                {
                    HtmlBuilderFactory.Paragraph.Cell(writer, "index", "..\\index.html");
                    HtmlBuilderFactory.H1.Cell(writer, $"Domain: {domain}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method.FullName);
                    });

                    if (!string.IsNullOrWhiteSpace(embeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                });
            });
        }
    }
}