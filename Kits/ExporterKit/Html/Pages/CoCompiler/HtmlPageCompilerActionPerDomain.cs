using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Page;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

public class HtmlPageCompilerActionPerDomain : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public HtmlPageCompilerActionPerDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
    }

    // context: html, build
    public async Task CompileAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainPage.Build ---", LogLevelNode.None);

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer>>
        {
            TabsheetFactory.ActionPerDomainClassesTabRegistration(exportOptions),
            TabsheetFactory.ActionPerDomainMethodsTabRegistration(),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer>(dataset, tabbedPageBuilder, (cellData) => $"composite_{cellData.ContextKey.Action}_{cellData.ContextKey.Domain}.html");
        builder.Build();
    }
}