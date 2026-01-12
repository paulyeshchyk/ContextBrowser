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

// context: uml, build
// pattern: Builder
public abstract class UmlDiagramCompilerActionPerDomainPackageBase : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;
    protected readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    protected readonly IAppOptionsStore _optionsStore;
    protected readonly INamingProcessor _namingProcessor;
    protected readonly UmlClassRendererActionPerDomainPackageMethod _renderer;

    protected UmlDiagramCompilerActionPerDomainPackageBase(
        IAppLogger<AppLevel> logger,
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider,
        IAppOptionsStore optionsStore,
        INamingProcessor namingProcessor,
        IUmlUrlBuilder umlUrlBuilder,
        UmlClassRendererActionPerDomainPackageMethod renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _renderer = renderer;
    }

    public abstract Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken);

    protected abstract Task BuildPackageAsync(List<ContextInfo> items, string groupName, CancellationToken cancellationToken);

    protected async Task ExportAsync(List<ContextInfo> contextInfoList, string outputPath, string diagramId, CancellationToken cancellationToken)
    {
        var renderResult = await _renderer.RenderAsync(contextInfoList, cancellationToken).ConfigureAwait(false);

        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return;
        }

        diagram.DiagramId = diagramId;
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetAllowMixing();
        diagram.SetSeparator("none");

        await diagram.WriteToFileAsync(outputPath, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);
    }
}
