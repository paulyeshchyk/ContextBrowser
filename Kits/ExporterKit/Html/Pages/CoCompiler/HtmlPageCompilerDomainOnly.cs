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
using ExporterKit.Html.Containers;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Builders.Core;
using HtmlKit.Document;
using HtmlKit.Model;
using HtmlKit.Model.Containers;
using HtmlKit.Page;
using LoggerKit;
using TensorKit.Factories;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;
using UmlKit.Infrastructure.Options;

namespace HtmlKit.Page.Compiler;

// context: contextInfo, build, html
public class HtmlPageCompilerDomainOnly<TDataTensor> : IHtmlPageCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IHtmlTensorWriter<MethodListTensor<TDataTensor>> _matrixWriter;
    private readonly ITensorFactory<MethodListTensor<TDataTensor>> _keyFactory;

    public HtmlPageCompilerDomainOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore, IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _matrixWriter = matrixWriter;
        _keyFactory = keyFactory;
    }

    // context: contextInfo, build, html
    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- DomainOnly.Build ---", LogLevelNode.None));

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var registrations = new List<IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>>>
        {
            TabsheetFactory<TDataTensor>.DomainOnlyClassesTabsheetRegistration(exportOptions),
            TabsheetFactory<TDataTensor>.DomainOnlyMethodsTabsheetRegistration(_matrixWriter, _keyFactory),
            TabsheetFactory<TDataTensor>.DomainOnlySequence(exportOptions),
            TabsheetFactory<TDataTensor>.DomainOnlyStates(exportOptions),
            TabsheetFactory<TDataTensor>.DomainOnlyMindmap(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextInfoKeyContainerTensor<TDataTensor>>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextInfoKeyContainerTensor<TDataTensor>>(exportOptions, tabsheetDataProvider);
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var builder = new HtmlPageWithTabsEntityListBuilder<ContextInfoKeyContainerTensor<TDataTensor>, TDataTensor>(dataset, tabbedPageBuilder, (cellData) => $"composite_domain_{cellData.ContextKey.Domain}.html");
        await builder.BuildAsync(cancellationToken);
    }
}