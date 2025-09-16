using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using ExporterKit.Infrastucture;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using LoggerKit;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

// context: contextInfo, build, html
public class HtmlPageCompilerActionOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoMapperFactory _contextInfoMapperContainer;
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public HtmlPageCompilerActionOnly(IAppLogger<AppLevel> logger, IContextInfoMapperFactory contextInfoMapperContainer, IContextInfoDatasetProvider datasetProvider)
    {
        _logger = logger;
        _contextInfoMapperContainer = contextInfoMapperContainer;
        _datasetProvider = datasetProvider;
    }

    // context: contextInfo, build, html
    public async Task CompileAsync(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- ActionOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer>>
        {
            TabsheetFactory.ActionOnlyClassesTabRegistration(exportOptions),
            TabsheetFactory.MethodsTabRegistration(),
            TabsheetFactory.StatesTabRegistration(exportOptions),
            TabsheetFactory.SequenceTabRegistration(exportOptions),
        };

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer>(exportOptions, tabsheetDataProvider);
        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer>(dataset, tabbedPageBuilder, (cellData) => $"composite_action_{cellData.ContextKey.Action}.html");
        builder.Build();
    }
}
