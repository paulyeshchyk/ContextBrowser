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
using TensorKit.Model;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

// context: contextInfo, build, html
public class HtmlPageCompilerActionOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoMapperFactory<DomainPerActionTensor> _contextInfoMapperContainer;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public HtmlPageCompilerActionOnly(
        IAppLogger<AppLevel> logger, 
        IContextInfoMapperFactory<DomainPerActionTensor> contextInfoMapperContainer, 
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, 
        IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _contextInfoMapperContainer = contextInfoMapperContainer;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    // context: contextInfo, build, html
    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- ActionOnly.Build ---", LogLevelNode.None));

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>>>
        {
            TabsheetFactory.ActionOnlyClassesTabRegistration(exportOptions),
            TabsheetFactory.MethodsTabRegistration(),
            TabsheetFactory.StatesTabRegistration(exportOptions),
            TabsheetFactory.SequenceTabRegistration(exportOptions),
        };

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer<DomainPerActionTensor>>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer<DomainPerActionTensor>>(exportOptions, tabsheetDataProvider);
        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer<DomainPerActionTensor>, DomainPerActionTensor>(dataset, tabbedPageBuilder, (cellData) => $"composite_action_{cellData.ContextKey.Action}.html");
        await builder.BuildAsync(cancellationToken);
    }
}
