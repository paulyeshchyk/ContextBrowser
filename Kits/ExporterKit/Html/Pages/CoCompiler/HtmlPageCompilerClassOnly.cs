using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using HtmlKit.Builders.Page.Tabs;
using HtmlKit.Builders.Page.Tabs.CoBuilders;
using HtmlKit.Model.Containers;
using LoggerKit;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler;

// context: ContextInfo, build, html
public class HtmlPageCompilerClassOnly<TDataTensor> : IHtmlPageCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;

    public HtmlPageCompilerClassOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
    }

    // context: ContextInfo, build, html
    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- DomainOnly.Build ---", LogLevelNode.None));

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var registrations = new List<IHtmlTabRegistration<ContextInfoKeyContainerEntityName>>
        {
            TabsheetFactory<TDataTensor>.DomainClassesTabRegistration(exportOptions, _namingProcessor),
            TabsheetFactory<TDataTensor>.DomainMindmapTabRegistration(exportOptions, _namingProcessor),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextInfoKeyContainerEntityName>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextInfoKeyContainerEntityName>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var builder = new HtmlPageWithTabsEntityBuilder<ContextInfoKeyContainerEntityName, TDataTensor>(dataset, tabbedPageBuilder, (contextInfo) => _namingProcessor.ClassOnlyHtmlFilename(contextInfo));
        await builder.BuildAsync(cancellationToken).ConfigureAwait(false);
    }
}