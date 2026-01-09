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
using ExporterKit.Infrastucture;
using ExporterKit.Uml.Builder;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.DiagramCompiler;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerPackageMethodPerActionDomain : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlDiagramCompilerPackageMethodPerActionDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
    }

    //context: build, uml
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile PackageMethodPerAction");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();
        var taskList = new List<Task?>();
        taskList.AddRange(CompileDomainGroupAsync(dataset, exportOptions, diagramBuilderOptions, _namingProcessor, _umlUrlBuilder, cancellationToken));
        taskList.Add(CompileNoDomainGroupAsync(dataset, exportOptions, diagramBuilderOptions, _namingProcessor, _umlUrlBuilder, cancellationToken));

        taskList.AddRange(CompileActionGroupAsync(dataset, exportOptions, diagramBuilderOptions, _namingProcessor, _umlUrlBuilder, cancellationToken));
        taskList.Add(CompileNoActionGroupAsync(dataset, exportOptions, diagramBuilderOptions, _namingProcessor, _umlUrlBuilder, cancellationToken));

        await Task.WhenAll(taskList.Where(t => t != null).Cast<Task>());
        return new Dictionary<ILabeledValue, bool>();
    }

    private static IEnumerable<Task?> CompileActionGroupAsync(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, CancellationToken cancellationToken)
    {
        var actionContexts = contextInfoDataset
            .SelectMany(cell => cell.Value.Select(context => (action: cell.Key.Action, context: context)))
            .GroupBy(item => item.action)
            .Where(group => !string.IsNullOrWhiteSpace(group.Key))
            .ToList();

        return actionContexts.Select(group =>
        {
            var action = group.Key;
            var contexts = group.Select(item => item.context).Distinct().ToList();

            if (contexts.Count == 0)
                return null;
            return BuildPackageActionAsync(contexts, exportOptions, diagramBuilderOptions, namingProcessor, umlUrlBuilder, action, cancellationToken);
        });
    }

    private static Task CompileNoActionGroupAsync(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, CancellationToken cancellationToken)
    {
        var actionContexts = contextInfoDataset
                .SelectMany(cell => cell.Value)
                .Where(context => string.IsNullOrWhiteSpace(context.Action))
                .ToList();

        return BuildPackageActionAsync(actionContexts, exportOptions, diagramBuilderOptions, namingProcessor, umlUrlBuilder, "NoAction", cancellationToken);
    }

    private static IEnumerable<Task> CompileDomainGroupAsync(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, CancellationToken cancellationToken)
    {
        var domainContexts = contextInfoDataset
            .SelectMany(cell => cell.Value)
            .SelectMany(context => context.Domains, (context, domain) => new { Domain = domain, Context = context }) // Разворачиваем ContextInfo по всем его доменам
            .Where(item => !string.IsNullOrWhiteSpace(item.Domain))
            .GroupBy(item => item.Domain)
            .ToList();

        return domainContexts.Select(group =>
        {
            var domain = group.Key;

            var contexts = group.Select(item => item.Context).Distinct().ToList();

            return BuildPackageDomainAsync(contexts, exportOptions, diagramBuilderOptions, namingProcessor, umlUrlBuilder, domain, cancellationToken);
        });
    }

    private static Task? CompileNoDomainGroupAsync(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, CancellationToken cancellationToken)
    {
        var domainContexts = contextInfoDataset
            .SelectMany(cell => cell.Value)
            .Where(context => context.Domains.Count == 0)
            .ToList();

        if (domainContexts.Count == 0)
            return null;

        // Вместо группировки - просто передаём список
        return BuildPackageDomainAsync(domainContexts, exportOptions, diagramBuilderOptions, namingProcessor, umlUrlBuilder, "NoDomain", cancellationToken);
    }

    private static Task BuildPackageDomainAsync(List<ContextInfo> items, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, string domain, CancellationToken cancellationToken)
    {
        var fileName = namingProcessor.ClassDomainPumlFilename(domain);
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        var actionDiagramId = namingProcessor.ClassDomainDiagramId(domain);

        return BuildPackageDiagramAsync(diagramBuilderOptions, namingProcessor, umlUrlBuilder, items, actionOutputPath, actionDiagramId, cancellationToken);
    }

    private static Task BuildPackageActionAsync(List<ContextInfo> items, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, string action, CancellationToken cancellationToken)
    {
        var puml = namingProcessor.ClassActionPumlFilename(action);
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, puml);
        var actionDiagramId = namingProcessor.ClassActionDiagramId(action);

        return BuildPackageDiagramAsync(diagramBuilderOptions, namingProcessor, umlUrlBuilder, items, actionOutputPath, actionDiagramId, cancellationToken);
    }

    private static async Task BuildPackageDiagramAsync(DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, List<ContextInfo> items, string outputPath, string diagramId, CancellationToken cancellationToken)
    {
        // Создаем новую диаграмму
        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetAllowMixing();
        diagram.SetSeparator("none");

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(items,
        [
            UmlDiagramMaxNamelengthExtractorType.@method,
            UmlDiagramMaxNamelengthExtractorType.@namespace,
            UmlDiagramMaxNamelengthExtractorType.entity,
            UmlDiagramMaxNamelengthExtractorType.property
        ]);

        var classesonly = items.Where(item => item.ElementType == ContextInfoElementType.@class).ToList();
        var methsonly = items.Where(item => item.ElementType == ContextInfoElementType.@method).ToList();
        var propsonly = items.Where(item => item.ElementType == ContextInfoElementType.property).ToList();
        var propssonlyClassOwners = items.Where(item => item.ElementType == ContextInfoElementType.property).Select(i => i.ClassOwner).ToList();
        var methodsonlyClassOwners = items.Where(item => item.ElementType == ContextInfoElementType.@method).Select(i => i.ClassOwner).ToList();

        var classes = classesonly.Concat(methodsonlyClassOwners)
                                 .Concat(propssonlyClassOwners)
                                 .Where(c => c != null && c is ContextInfo)
                                 .Cast<ContextInfo>()
                                 .Where(c => !string.IsNullOrWhiteSpace(c.Namespace))
                                 .Distinct()
                                 .ToList();

        var namespaces = classes.Select(c => c.Namespace).Distinct();

        foreach (var ns in namespaces)
        {
            var umlPackage = PumlBuilderHelper.BuildUmlPackage(umlUrlBuilder, ns);

            var classesInNamespace = classes.Where(c => c.Namespace == ns).Distinct().ToList();

            foreach (var contextInfo in classesInNamespace)
            {
                var umlClass = PumlBuilderHelper.BuildUmlEntityClass(contextInfo, methsonly, propsonly, maxLength, namingProcessor);

                umlPackage.Add(umlClass);
            }

            diagram.Add(umlPackage);
            diagram.AddRelations(PumlBuilderSquaredLayout.Build(classesInNamespace.Select(cls => cls.FullName.AlphanumericOnly())));
        }

        diagram.AddRelations(PumlBuilderSquaredLayout.Build(namespaces.Select(ns => ns.AlphanumericOnly())));

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        await diagram.WriteToFileAsync(outputPath, writeOptons, cancellationToken).ConfigureAwait(false);

    }
}