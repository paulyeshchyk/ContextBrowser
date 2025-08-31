using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.DiagramCompiler;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Puml;
using LoggerKit;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

// context: html, dimension, build
public class InjectorUmlStateDomainDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public InjectorUmlStateDomainDiagramCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: html, dimension, build
    public void Build(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {

        // Инициализация генераторов
        var stateGenerator = new UmlStateDomainDiagramCompiler(_logger);

        // Генерация диаграмм
        var _renderedStates = stateGenerator.Compile(contextInfoDataSet, contextClassifier, exportOptions, diagramBuilderOptions);

        var allDomains = contextInfoDataSet.ContextInfoData.GetDomains().Distinct();

        foreach (var domain in allDomains)
        {
            var methods = contextInfoDataSet.ContextInfoData.GetMethodsByDomain(domain: domain).ToList();

            PumlHtmlInjection? puml = null;
            if (_renderedStates.TryGetValue(domain, out var rendered_state) && rendered_state == true)
            {
                puml = PumlInjector.InjectDomainSequenceEmbeddedHtml(domain, exportOptions);
            }

            HtmlSequenceDomainPageBuilder.Build(domain, methods, puml, exportOptions);
        }
    }
}
