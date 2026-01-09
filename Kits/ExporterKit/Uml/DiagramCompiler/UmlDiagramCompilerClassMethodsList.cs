using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Service;
using ExporterKit.Uml.Builder;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.DiagramCompiler;

// context: uml, links, build
// pattern: Builder
public class UmlDiagramCompilerClassMethodsList : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;

    public UmlDiagramCompilerClassMethodsList(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, IContextInfoManager<ContextInfo> contextInfoManager)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _contextInfoManager = contextInfoManager;

    }

    // context: build, uml, links
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassMethodList");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "methodlinks.puml");

        var methods = dataset.GetAll()
                .Where(e => e.ElementType == ContextInfoElementType.method)
                .ToList();

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(methods, [UmlDiagramMaxNamelengthExtractorType.@method]);

        var diagramId = $"methods_only_{outputPath}".AlphanumericOnly();
        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);

        foreach (var method in methods)
        {
            var comp = PumlBuilderHelper.BuildUmlComponent(maxLength, method);
            diagram.Add(comp);
        }

        foreach (var method in methods)
        {
            var references = _contextInfoManager.GetReferencesSortedByInvocation(method);
            foreach (var callee in references)
            {
                if (methods.All(m => m.Name != callee.Name))
                    continue;

                var transitionState = PumlBuilderHelper.BuildUmlTransitionState(method, callee);
                diagram.Add(transitionState);
            }
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        await diagram.WriteToFileAsync(outputPath, writeOptons, cancellationToken);

        return new Dictionary<ILabeledValue, bool>();
    }
}