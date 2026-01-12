using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.Mindmap;

// context: uml, build
public class UmlDiagramCompilerNamespaceOnly<TDataTensor> : IUmlDiagramCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly UmlClassRendererNamespace<TDataTensor> _renderer;

    public UmlDiagramCompilerNamespaceOnly(
        IAppLogger<AppLevel> logger, 
        IContextInfoDatasetProvider<TDataTensor> datasetProvider, 
        IAppOptionsStore optionsStore, 
        INamingProcessor namingProcessor, 
        IUmlUrlBuilder umlUrlBuilder, 
        UmlClassRendererNamespace<TDataTensor> renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _renderer = renderer;
    }

    //context: uml, build
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile Namespaces only");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);


        var namespaces = GetNamespaces(dataset).ToList();
        foreach (var nameSpace in namespaces)
        {
            await ExportNamespaceAsync(nameSpace: nameSpace, cancellationToken: cancellationToken);
        }
        return new Dictionary<ILabeledValue, bool>();
    }

    private static Func<string, IEnumerable<IContextInfo>> GetClassesForNamespace(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return (nameSpace) => contextInfoDataSet.GetAll()
            .Where(c => c.ElementType.IsEntityDefinition() || c.MethodOwnedByItSelf == true)
            .Where(c => c.Namespace == nameSpace);
    }

    private IEnumerable<string> GetNamespaces(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return contextInfoDataSet.GetAll().Select(c => c.Namespace).Distinct();
    }

    internal async Task ExportNamespaceAsync(string nameSpace, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
        var classesList = GetClassesForNamespace(dataset)(nameSpace).ToList();

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramId = _namingProcessor.NamespaceOnlyItemDiagramId(nameSpace);
        var diagramTitle = string.Format("Package diagram -> {0}", nameSpace);

        var renderResult = await _renderer.RenderAsync(nameSpace, classesList, cancellationToken).ConfigureAwait(false);

        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return;
        }
        diagram.DiagramId = diagramId;
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, _namingProcessor.NamespaceOnlyItemPumlFilename(nameSpace));
        await diagram.WriteToFileAsync(outputPath, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);
    }
}
