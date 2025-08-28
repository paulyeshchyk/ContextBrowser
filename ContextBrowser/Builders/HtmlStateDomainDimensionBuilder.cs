using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Puml;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

[Obsolete]
// context: contextInfo, build, html
public static class HtmlStateDomainDimensionBuilder
{
    // context: contextInfo, build, html
    public static void Build(IContextInfoDataset model, AppOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Cntx, "--- DimensionBuilder.Build ---");

        var builder = new HtmlStateDomainBuilder(
            model.ContextInfoData,
            model.ContextsList,
            contextClassifier,
            options.Export,
            options.DiagramBuilder,
            onWriteLog);

        builder.Build();
    }
}

// context: html, dimension, build
internal class HtmlStateDomainBuilder
{
    private readonly IContextInfoData _matrix;
    private readonly List<ContextInfo> _allContexts;
    private readonly ExportOptions _exportOptions;
    private readonly Dictionary<string, bool> _renderedStates;

    public HtmlStateDomainBuilder(IContextInfoData matrix, List<ContextInfo> allContexts, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, OnWriteLog? onWriteLog)
    {
        _matrix = matrix;
        _allContexts = allContexts;
        _exportOptions = exportOptions;

        // Инициализация генераторов
        var stateGenerator = new UmlStateDomainDiagramCompiler(_matrix, contextClassifier, exportOptions, options, onWriteLog);

        // Генерация диаграмм
        _renderedStates = stateGenerator.Generate(_allContexts);
    }

    // context: html, dimension, build
    public void Build()
    {
        var allDomains = _matrix.GetDomains().Distinct();

        foreach (var domain in allDomains)
        {
            var methods = _matrix.GetMethodsByDomain(domain: domain).ToList();

            PumlHtmlInjection? puml = null;
            if (_renderedStates.TryGetValue(domain, out var rendered_state) && rendered_state == true)
            {
                puml = PumlInjector.InjectDomainSequenceEmbeddedHtml(domain, _exportOptions);
            }

            HtmlSequenceDomainPageBuilder.Build(domain, methods, puml, _exportOptions);
        }
    }
}
