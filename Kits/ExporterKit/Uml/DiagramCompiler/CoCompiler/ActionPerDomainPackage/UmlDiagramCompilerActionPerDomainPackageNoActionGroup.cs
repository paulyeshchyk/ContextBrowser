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

public class UmlDiagramCompilerActionPerDomainPackageNoActionGroup : UmlDiagramCompilerActionPerDomainActionPackageBase
{
    public UmlDiagramCompilerActionPerDomainPackageNoActionGroup(
        IAppLogger<AppLevel> logger,
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider,
        IAppOptionsStore optionsStore,
        INamingProcessor namingProcessor,
        IUmlUrlBuilder umlUrlBuilder,
        UmlClassRendererActionPerDomainPackageMethod renderer)
        : base(logger, datasetProvider, optionsStore, namingProcessor, umlUrlBuilder, renderer)
    {
    }

    public override async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile NoActionGroup");
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var actionContexts = dataset
                .SelectMany(cell => cell.Value)
                .Where(context => string.IsNullOrWhiteSpace(context.Action))
                .ToList();

        await BuildPackageAsync(actionContexts, "NoAction", cancellationToken);

        return new Dictionary<ILabeledValue, bool>();
    }
}