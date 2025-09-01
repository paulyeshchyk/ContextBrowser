using System;
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

[Obsolete]
// context: contextInfo, build, html
public static class PumlStateDomainDimensionBuilder
{
    // context: contextInfo, build, html
    public static void Build(IContextInfoDataset model, AppOptions options, IContextClassifier contextClassifier, IAppLogger<AppLevel> _logger)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- DimensionBuilder.Build ---");

        var builder = new InjectorUmlStateDomainDiagramCompiler(

            _logger);

        builder.Build(model, contextClassifier, options.Export, options.DiagramBuilder);
    }
}
