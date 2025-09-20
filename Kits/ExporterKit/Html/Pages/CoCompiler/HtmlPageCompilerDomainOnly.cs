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
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;
using UmlKit.Infrastructure.Options;

namespace HtmlKit.Page.Compiler;

// context: contextInfo, build, html
public class HtmlPageCompilerDomainOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public HtmlPageCompilerDomainOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore)
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

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>>>
        {
            TabsheetFactory.DomainOnlyClassesTabsheetRegistration(exportOptions),
            TabsheetFactory.DomainOnlyMethodsTabsheetRegistration(),
            TabsheetFactory.DomainOnlySequence(exportOptions),
            TabsheetFactory.DomainOnlyStates(exportOptions),
            TabsheetFactory.DomainOnlyMindmap(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer<DomainPerActionTensor>>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer<DomainPerActionTensor>>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer<DomainPerActionTensor>, DomainPerActionTensor>(dataset, tabbedPageBuilder, (cellData) => $"composite_domain_{cellData.ContextKey.Domain}.html");
        await builder.BuildAsync(cancellationToken);
    }
}