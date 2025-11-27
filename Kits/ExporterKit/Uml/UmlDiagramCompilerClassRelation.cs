using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml.Exporters;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

// context: uml, links, build
// pattern: Builder
public class UmlDiagramCompilerClassRelation : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerClassRelation(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    //context: build, uml, links
    public async Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassRelation");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var contextInfoList = dataset.GetAll().ToList();
        var linkGenerator = new ContextInfoDataLinkGenerator(_optionsStore);
        var links = linkGenerator.Generate(contextInfoList);

        UmlDiagramExporter_4_links.Export(exportOptions, diagramBuilderOptions, links);
        return new Dictionary<object, bool>();
    }
}
