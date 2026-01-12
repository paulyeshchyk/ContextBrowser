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

public class UmlDiagramCompilerActionPerDomainPackageActionGroup : UmlDiagramCompilerActionPerDomainActionPackageBase
{
    public UmlDiagramCompilerActionPerDomainPackageActionGroup(
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
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ActionGroup");
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var actionContexts = dataset
            .SelectMany(cell => cell.Value.Select(context => (action: cell.Key.Action, context: context)))
            .GroupBy(item => item.action)
            .Where(group => !string.IsNullOrWhiteSpace(group.Key))
            .ToList();

        var taskList = actionContexts.Select(group =>
        {
            var action = group.Key;
            var contexts = group.Select(item => item.context).Distinct().ToList();

            return BuildPackageAsync(contexts, action, cancellationToken);
        })
        .Where(t => t != null).Cast<Task>()
        .ToList();

        await Task.WhenAll(taskList);
        return new Dictionary<ILabeledValue, bool>();
    }
}
