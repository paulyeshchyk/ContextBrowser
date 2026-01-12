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

public abstract class UmlDiagramCompilerActionPerDomainDomainPackageBase : UmlDiagramCompilerActionPerDomainPackageBase
{
    protected UmlDiagramCompilerActionPerDomainDomainPackageBase(
        IAppLogger<AppLevel> logger,
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider,
        IAppOptionsStore optionsStore,
        INamingProcessor namingProcessor,
        IUmlUrlBuilder umlUrlBuilder,
        UmlClassRendererActionPerDomainPackageMethod renderer)
        : base(logger, datasetProvider, optionsStore, namingProcessor, umlUrlBuilder, renderer)
    {
    }

    protected override Task BuildPackageAsync(List<ContextInfo> items, string domain, CancellationToken cancellationToken)
    {
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var fileName = _namingProcessor.ClassDomainPumlFilename(domain);
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        var actionDiagramId = _namingProcessor.ClassDomainDiagramId(domain);

        return ExportAsync(items, actionOutputPath, actionDiagramId, cancellationToken);
    }
}
