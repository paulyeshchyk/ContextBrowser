using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

public class HtmlPageCompilerClassOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public HtmlPageCompilerClassOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    // context: contextInfo, build, html
    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- DomainOnly.Build ---", LogLevelNode.None));

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var registrations = new List<IHtmlTabRegistration<EntitynameContainer>>
        {
            TabsheetFactory.ClassesTabRegistration(exportOptions),
            TabsheetFactory.MindmapTabRegistration(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<EntitynameContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<EntitynameContainer>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsEntityBuilder<EntitynameContainer>(dataset, tabbedPageBuilder, (contextInfo) => $"class_only_{contextInfo.AlphanumericOnly()}.html");
        await builder.BuildAsync(cancellationToken);
    }
}