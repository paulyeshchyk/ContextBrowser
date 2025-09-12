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
using UmlKit.Infrastructure.Options;

namespace HtmlKit.Page.Compiler;

// context: contextInfo, build, html
public class HtmlPageCompilerDomainOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public HtmlPageCompilerDomainOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
    }

    // context: contextInfo, build, html
    public async Task CompileAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- DomainOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer>>
        {
            TabsheetFactory.DomainOnlyClassesTabsheetRegistration(exportOptions),
            TabsheetFactory.DomainOnlyMethodsTabsheetRegistration(),
            TabsheetFactory.DomainOnlySequence(exportOptions),
            TabsheetFactory.DomainOnlyStates(exportOptions),
            TabsheetFactory.DomainOnlyMindmap(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer>(dataset, tabbedPageBuilder, (cellData) => $"composite_domain_{cellData.ContextKey.Domain}.html");
        builder.Build();
    }
}