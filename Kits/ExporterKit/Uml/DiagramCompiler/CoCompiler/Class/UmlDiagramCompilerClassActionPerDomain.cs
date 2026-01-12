using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.Class;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerClassActionPerDomain<TDataTensor> : IUmlDiagramCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly UmlClassRendererActionPerDomainClass _renderer;

    public UmlDiagramCompilerClassActionPerDomain(
        IAppLogger<AppLevel> logger, 
        IContextInfoDatasetProvider<TDataTensor> datasetProvider, 
        IAppOptionsStore optionsStore, 
        INamingProcessor namingProcessor, 
        IUmlUrlBuilder umlUrlBuilder, 
        UmlClassRendererActionPerDomainClass renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _renderer = renderer;
    }

    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassActionPerDomain");

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
        var tasks = dataset.Select(async e => await CompileItemAsync(e, exportOptions, cancellationToken).ConfigureAwait(false));
        await Task.WhenAll(tasks);
        return new Dictionary<ILabeledValue, bool>();
    }

    //context: uml, build, heatmap, directory
    internal async Task CompileItemAsync(KeyValuePair<TDataTensor, List<ContextInfo>> cell, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var contextInfoKey = cell.Key;
        var contextInfoList = cell.Value.Distinct().Where(c => (c.ElementType.IsEntityDefinition())).ToList();

        var pumlClass = _namingProcessor.ClassActionDomainPumlFilename(contextInfoKey.Action, contextInfoKey.Domain);
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, pumlClass);

        var diagramId = _namingProcessor.ClassActionDomainDiagramId(contextInfoKey.Action, contextInfoKey.Domain).AlphanumericOnly();
        var diagramTitle = _namingProcessor.ClassActionDomainDiagramTitle(contextInfoKey.Action, contextInfoKey.Domain);

        var renderResult = await _renderer.RenderAsync(contextInfoList, cancellationToken).ConfigureAwait(false);

        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return;
        }
        diagram.DiagramId = diagramId;
        diagram.SetTitle(diagramTitle);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        await diagram.WriteToFileAsync(fileName, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);
    }
}
