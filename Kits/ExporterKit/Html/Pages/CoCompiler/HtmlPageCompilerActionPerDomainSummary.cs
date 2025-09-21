using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Containers;
using ExporterKit.Html.Pages.CoCompiler;
using ExporterKit.Html.Pages.MatrixCellSummary;
using HtmlKit;
using HtmlKit.Builders.Core;
using HtmlKit.Document;
using HtmlKit.Model;
using HtmlKit.Model.Containers;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using LoggerKit;
using TensorKit.Factories;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Html.Pages.MatrixCellSummary;

public class HtmlPageCompilerActionPerDomainSummary : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IHtmlTensorWriter<MethodListTensor> _matrixWriter;
    private readonly ITensorFactory<MethodListTensor> _keyFactory;

    public HtmlPageCompilerActionPerDomainSummary(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, IHtmlTensorWriter<MethodListTensor> matrixWriter, ITensorFactory<MethodListTensor> keyFactory)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _matrixWriter = matrixWriter;
        _keyFactory = keyFactory;
    }

    // context: html, build

    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainPage.Build ---", LogLevelNode.None);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var registrations = new List<IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>>>
        {
            TabsheetFactory.DomainOnlyClassesTabRegistration(exportOptions),
            TabsheetFactory.DomainOnlyMethodsTabRegistration(_matrixWriter, _keyFactory),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextInfoKeyContainerTensor<DomainPerActionTensor>>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextInfoKeyContainerTensor<DomainPerActionTensor>>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextInfoKeyContainerTensor<DomainPerActionTensor>, DomainPerActionTensor>(dataset, tabbedPageBuilder, (_) => $"summary.html");
        await builder.BuildAsync(cancellationToken);
    }
}
