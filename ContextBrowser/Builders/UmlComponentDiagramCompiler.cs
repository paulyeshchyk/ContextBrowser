using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.DiagramCompiler;
using LoggerKit;
using UmlKit.Exporter;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

public class UmlComponentDiagramCompiler : IDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlComponentDiagramCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: uml, build

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ComponentDiagram.Build ---");
        UmlContextComponentDiagram.Build(contextInfoDataSet.ContextInfoData, exportOptions, diagramBuilderOptions);
        return new() { };
    }
}
