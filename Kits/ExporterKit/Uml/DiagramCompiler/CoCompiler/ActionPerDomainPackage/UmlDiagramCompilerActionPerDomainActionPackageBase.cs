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
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ExporterKit.Uml.DiagramCompiler.ActionPerDomainPackage;

public abstract class UmlDiagramCompilerActionPerDomainActionPackageBase : UmlDiagramCompilerActionPerDomainPackageBase
{
    protected UmlDiagramCompilerActionPerDomainActionPackageBase(
        IAppLogger<AppLevel> logger,
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider,
        IAppOptionsStore optionsStore,
        INamingProcessor namingProcessor,
        IUmlUrlBuilder umlUrlBuilder,
        UmlClassRendererActionPerDomainPackageMethod renderer)
        : base(logger, datasetProvider, optionsStore, namingProcessor, umlUrlBuilder, renderer)
    {
    }

    protected override Task BuildPackageAsync(List<ContextInfo> items, string action, CancellationToken cancellationToken)
    {
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var fileName = _namingProcessor.ClassActionPumlFilename(action);
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        var actionDiagramId = _namingProcessor.ClassActionDiagramId(action);

        return ExportAsync(items, actionOutputPath, actionDiagramId, cancellationToken);
    }
}
