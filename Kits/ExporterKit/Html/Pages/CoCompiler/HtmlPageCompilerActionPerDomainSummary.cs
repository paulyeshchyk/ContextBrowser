using System;
using System.Collections.Generic;
using System.Linq;
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

    public HtmlPageCompilerActionPerDomainSummary(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: html, build

    public void Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainPage.Build ---", LogLevelNode.None);

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer>>
        {
            TabsheetFactory.DomainOnlyClassesTabRegistration(exportOptions),
            TabsheetFactory.DomainOnlyMethodsTabRegistration(),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer>(exportOptions, tabsheetDataProvider);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer>(contextInfoDataset, tabbedPageBuilder, (_) => $"summary.html");
        builder.Build();
    }
}
