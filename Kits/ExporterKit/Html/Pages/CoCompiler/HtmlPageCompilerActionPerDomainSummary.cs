using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using ExporterKit.Html.Pages.MatrixCellSummary;
using HtmlKit;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using LoggerKit;

namespace ExporterKit.Html.Pages.MatrixCellSummary;

public class HtmlPageCompilerActionPerDomainSummary : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public HtmlPageCompilerActionPerDomainSummary(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
    }

    // context: html, build

    public async Task CompileAsync(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainPage.Build ---", LogLevelNode.None);

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer>>
        {
            TabsheetFactory.DomainOnlyClassesTabRegistration(exportOptions),
            TabsheetFactory.DomainOnlyMethodsTabRegistration(),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer>(dataset, tabbedPageBuilder, (_) => $"summary.html");
        builder.Build();
    }
}
