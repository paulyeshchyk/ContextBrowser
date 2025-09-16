using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Model;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

public class HtmlPageCompilerNamespaceOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public HtmlPageCompilerNamespaceOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
    }

    // context: contextInfo, build, html
    public async Task CompileAsync(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- NamespaceOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration<NamespacenameContainer>>
        {
            TabsheetFactory.NamespaceTabRegistration(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<NamespacenameContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<NamespacenameContainer>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsNamespaceEntityBuilder<NamespacenameContainer>(dataset, tabbedPageBuilder, (ns) => $"namespace_only_{ns.AlphanumericOnly()}.html");
        builder.Build();
    }
}