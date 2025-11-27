using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using ExporterKit.Uml.Exporters;
using GraphKit.Walkers;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders.Url;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

public class UmlDiagramCompilerMindmapAction : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerMindmapAction(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    // context: build, uml
    public async Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "Compile Mindmap Action");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll();
        var distinctAction = elements.Select(e => e.Action).Distinct().Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).Select(s => s.Split(';')).SelectMany(s => s).Distinct();

        foreach (var action in distinctAction)
        {
            UmlDiagramExporterMindMapAction.Export(dataset, exportOptions, diagramBuilderOptions, action);
        }

        return new Dictionary<object, bool>();
    }
}
