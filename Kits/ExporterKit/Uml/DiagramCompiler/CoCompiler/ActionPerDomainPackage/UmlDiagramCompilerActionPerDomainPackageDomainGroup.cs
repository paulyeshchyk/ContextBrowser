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

public class UmlDiagramCompilerActionPerDomainPackageDomainGroup : UmlDiagramCompilerActionPerDomainDomainPackageBase
{
    public UmlDiagramCompilerActionPerDomainPackageDomainGroup(
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
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile DomainGroup");
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var domainContexts = dataset
            .SelectMany(cell => cell.Value)
            .SelectMany(context => context.Domains, (context, domain) => new { Domain = domain, Context = context })
            .Where(item => !string.IsNullOrWhiteSpace(item.Domain))
            .GroupBy(item => item.Domain)
            .ToList();

        var taskList = domainContexts.Select(group =>
        {
            var domain = group.Key;
            var contexts = group.Select(item => item.Context).Distinct().ToList();

            return BuildPackageAsync(contexts, domain, cancellationToken);
        }).ToList();

        await Task.WhenAll(taskList);
        return new Dictionary<ILabeledValue, bool>();
    }
}
