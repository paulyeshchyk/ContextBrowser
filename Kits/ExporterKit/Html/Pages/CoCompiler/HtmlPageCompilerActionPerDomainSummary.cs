using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Html.Containers;
using HtmlKit.Builders.Page.Tabs;
using HtmlKit.Builders.Page.Tabs.CoBuilders;
using HtmlKit.Document;
using HtmlKit.Model.Containers;
using LoggerKit;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler;

public class HtmlPageCompilerActionPerDomainSummary<TDataTensor> : IHtmlPageCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IHtmlTensorWriter<MethodListTensor<TDataTensor>> _matrixWriter;
    private readonly ITensorFactory<MethodListTensor<TDataTensor>> _keyFactory;
    private readonly INamingProcessor _namingProcessor;

    public HtmlPageCompilerActionPerDomainSummary(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore, IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory, INamingProcessor namingProcessor)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _matrixWriter = matrixWriter;
        _keyFactory = keyFactory;
        _namingProcessor = namingProcessor;

    }

    // context: html, build

    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainPage.Build ---", LogLevelNode.None);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var registrations = new List<IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>>>
        {
            TabsheetFactory<TDataTensor>.DomainOnlyClassesTabRegistration(exportOptions, _namingProcessor),
            TabsheetFactory<TDataTensor>.DomainOnlyMethodsTabRegistration(_matrixWriter, _keyFactory, _namingProcessor),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextInfoKeyContainerTensor<TDataTensor>>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextInfoKeyContainerTensor<TDataTensor>>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextInfoKeyContainerTensor<TDataTensor>, TDataTensor>(
            dataset,
            tabbedPageBuilder,
            (_) => $"summary.html",
            (_) => "summary"
            );
        await builder.BuildAsync(cancellationToken).ConfigureAwait(false);
    }
}
