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
using ExporterKit.Uml.Exporters;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.Uml;

public class UmlDiagramCompilerMindmapAction : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;


    public UmlDiagramCompilerMindmapAction(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;

    }

    // context: build, uml
    public async Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "Compile Mindmap Action");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll();
        var distinctAction = elements.Select(e => e.Action).Distinct().Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).Select(s => s.Split(';')).SelectMany(s => s).Distinct();

        foreach (var action in distinctAction)
        {
            UmlDiagramExporterMindMapAction.Export(dataset, exportOptions, diagramBuilderOptions, action, _namingProcessor, _umlUrlBuilder);
        }

        return new Dictionary<object, bool>();
    }
}
