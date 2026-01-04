using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Html.Containers;
using ExporterKit.Infrastucture;
using HtmlKit.Builders.Page.Tabs;
using HtmlKit.Builders.Page.Tabs.CoBuilders;
using HtmlKit.Document;
using HtmlKit.Model.Containers;
using LoggerKit;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler;

// context: ContextInfo, build, html
public class HtmlPageCompilerActionOnly<TDataTensor> : IHtmlPageCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoMapperFactory<TDataTensor> _contextInfoMapperContainer;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IHtmlTensorWriter<MethodListTensor<TDataTensor>> _matrixWriter;
    private readonly ITensorFactory<MethodListTensor<TDataTensor>> _keyFactory;
    private readonly INamingProcessor _namingProcessor;

    public HtmlPageCompilerActionOnly(
        IAppLogger<AppLevel> logger,
        IContextInfoMapperFactory<TDataTensor> contextInfoMapperContainer,
        IContextInfoDatasetProvider<TDataTensor> datasetProvider,
        IAppOptionsStore optionsStore,
        IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter,
        ITensorFactory<MethodListTensor<TDataTensor>> keyFactory,
        INamingProcessor namingProcessor)
    {
        _logger = logger;
        _contextInfoMapperContainer = contextInfoMapperContainer;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _matrixWriter = matrixWriter;
        _keyFactory = keyFactory;
        _namingProcessor = namingProcessor;

    }

    // context: ContextInfo, build, html
    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- ActionOnly.Build ---", LogLevelNode.None));

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var registrations = new List<IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>>>
        {
            TabsheetFactory<TDataTensor>.ActionOnlyClassesTabRegistration(exportOptions, _namingProcessor),
            TabsheetFactory<TDataTensor>.MethodsTabRegistration(_matrixWriter, _keyFactory, _namingProcessor),
            TabsheetFactory<TDataTensor>.StatesTabRegistration(exportOptions, _namingProcessor),
            TabsheetFactory<TDataTensor>.SequenceTabRegistration(exportOptions, _namingProcessor),
            TabsheetFactory<TDataTensor>.ActionOnlyMindmap(exportOptions, _namingProcessor),
        };

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextInfoKeyContainerTensor<TDataTensor>>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextInfoKeyContainerTensor<TDataTensor>>(exportOptions, tabsheetDataProvider);
        var builder = new HtmlPageWithTabsEntityListBuilder<ContextInfoKeyContainerTensor<TDataTensor>, TDataTensor>(
            contextInfoDataset: dataset,
            tabbedPageBuilder: tabbedPageBuilder,
            onGetFileName: (cellData) => _namingProcessor.CompositeActionHtmlFile(cellData.ContextKey.Action),
            onGetTitle: (cellData) => string.Format("Action: {0} ", cellData.ContextKey.Action)
            );
        await builder.BuildAsync(cancellationToken).ConfigureAwait(false);
    }
}
