using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using ExporterKit.Infrastucture;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders.Url;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerPackageMethodPerActionDomain : IUmlDiagramCompiler
{
    private const string SParentheses = "()";
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;

    public UmlDiagramCompilerPackageMethodPerActionDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
    }

    //context: build, uml
    public async Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile PackageMethodPerAction");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        CompileDomainGroup(dataset, exportOptions, diagramBuilderOptions, _namingProcessor);
        CompileNoDomainGroup(dataset, exportOptions, diagramBuilderOptions, _namingProcessor);

        CompileActionGroup(dataset, exportOptions, diagramBuilderOptions, _namingProcessor);
        CompileNoActionGroup(dataset, exportOptions, diagramBuilderOptions, _namingProcessor);

        return new Dictionary<object, bool>();
    }

    private static void CompileActionGroup(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor _namingProcessor)
    {
        var actionContexts = contextInfoDataset
            .SelectMany(cell => cell.Value.Select(context => (action: cell.Key.Action, context: context)))
            .GroupBy(item => item.action)
            .Where(group => !string.IsNullOrWhiteSpace((string)group.Key));

        foreach (var group in actionContexts)
        {
            var action = (string)group.Key;
            var contexts = group.Select(item => item.context).Distinct().ToList();

            if (contexts.Any())
            {
                BuildPackageAction(contexts, exportOptions, diagramBuilderOptions, _namingProcessor, action, "?");
            }
        }
    }

    private static void CompileNoActionGroup(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor _namingProcessor)
    {
        var actionContexts = contextInfoDataset
                .SelectMany(cell => cell.Value)
                .Where(context => string.IsNullOrWhiteSpace(context.Action))
                .ToList();

        BuildPackageAction(actionContexts, exportOptions, diagramBuilderOptions, _namingProcessor, "NoAction", "?");
    }

    private static void CompileDomainGroup(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor _namingProcessor)
    {
        var domainContexts = contextInfoDataset
            .SelectMany(cell => cell.Value)
            .GroupBy(context => context.Domains.FirstOrDefault())
            .Where(group => group.Key != null);

        foreach (var group in domainContexts)
        {
            var domain = group.Key; // The unique domain string
            var contexts = group.ToList(); // The list of all ContextInfo for that domain
            if (!string.IsNullOrWhiteSpace(domain))
            {
                BuildPackageDomain(contexts, exportOptions, diagramBuilderOptions, _namingProcessor, domain, "?");
            }
            else
            {
            }
        }
    }

    private static void CompileNoDomainGroup(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor _namingProcessor)
    {
        var domainContexts = contextInfoDataset
            .SelectMany(cell => cell.Value)
            .Where(context => !context.Domains.Any())
            .ToList();

        if (domainContexts.Any())
        {
            // Вместо группировки - просто передаём список
            BuildPackageDomain(domainContexts, exportOptions, diagramBuilderOptions, _namingProcessor, "NoDomain", "?");
        }
    }

    private static void BuildPackageDomain(IEnumerable<ContextInfo> items, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor _namingProcessor, string domain, string blockLabel)
    {
        var fileName = _namingProcessor.ClassDomainPumlFilename(domain);
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        var actionDiagramId = _namingProcessor.ClassDomainDiagramId(domain);

        BuildPackageDiagram(diagramBuilderOptions, _namingProcessor, blockLabel, items, actionOutputPath, actionDiagramId);
    }

    private static void BuildPackageAction(IEnumerable<ContextInfo> items, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor _namingProcessor, string action, string blockLabel)
    {
        var puml = _namingProcessor.ClassActionPumlFilename(action);
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, puml);
        var actionDiagramId = _namingProcessor.ClassActionDiagramId(action);

        BuildPackageDiagram(diagramBuilderOptions, _namingProcessor, blockLabel, items, actionOutputPath, actionDiagramId);
    }

    private static void BuildPackageDiagram(DiagramBuilderOptions diagramBuilderOptions, INamingProcessor _namingProcessor, string blockLabel, IEnumerable<ContextInfo> items, string outputPath, string diagramId)
    {
        // Создаем новую диаграмму
        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetAllowMixing();
        diagram.SetSeparator("none");

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(items, new() { UmlDiagramMaxNamelengthExtractorType.@method, UmlDiagramMaxNamelengthExtractorType.@namespace, UmlDiagramMaxNamelengthExtractorType.entity, UmlDiagramMaxNamelengthExtractorType.property });

        var classesonly = items.Where(item => item.ElementType == ContextInfoElementType.@class).ToList();
        var methodsonly = items.Where(item => item.ElementType == ContextInfoElementType.@method).ToList();
        var propssonly = items.Where(item => item.ElementType == ContextInfoElementType.property).ToList();
        var propssonlyClassOwners = items.Where(item => item.ElementType == ContextInfoElementType.property).Select(i => i.ClassOwner).ToList();
        var methodsonlyClassOwners = items.Where(item => item.ElementType == ContextInfoElementType.@method).Select(i => i.ClassOwner).ToList();

        var classes = classesonly.Concat(methodsonlyClassOwners)
                                 .Concat(propssonlyClassOwners)
                                 .Cast<ContextInfo>().Distinct();

        var namespaces = classes.Select(c => c.Namespace).Where(ns => !string.IsNullOrWhiteSpace(ns)).Distinct().ToList();

        foreach (var ns in namespaces)
        {
            var namespaceUrl = UmlUrlBuilder.BuildNamespaceUrl(ns);
            var umlPackage = new UmlPackage(ns, alias: ns.AlphanumericOnly(), url: namespaceUrl);

            var classesInNamespace = classes.Where(c => c.Namespace == ns).Distinct().ToList();

            foreach (var cls in classesInNamespace)
            {
                string? htmlUrl = _namingProcessor.ClassOnlyHtmlFilename(cls.FullName);

                var entityType = ContextInfoExt.ConvertToUmlEntityType(cls.ElementType);
                var umlClass = new UmlEntity(entityType, cls.Name.PadRight(maxLength), cls.FullName.AlphanumericOnly(), url: htmlUrl);
                var propsList = propssonly.Where(p => p.ClassOwner?.FullName.Equals(cls?.FullName) ?? false).Distinct();
                foreach (var element in propsList)
                {
                    string? url = null;// UmlUrlBuilder.BuildUrl(element);
                    umlClass.Add(new UmlProperty(element.ShortName.PadRight(maxLength), visibility: UmlMemberVisibility.@public, url: url));
                }

                var methodList = methodsonly.Where(m => m.ClassOwner?.FullName.Equals(cls?.FullName) ?? false).Distinct();
                foreach (var element in methodList)
                {
                    string? url = null;//UmlUrlBuilder.BuildUrl(element);
                    umlClass.Add(new UmlMethod(element.ShortName + SParentheses.PadRight(maxLength), visibility: UmlMemberVisibility.@public, url: url));
                }

                umlPackage.Add(umlClass);
            }

            diagram.Add(umlPackage);
            diagram.AddRelations(UmlSquaredLayout.Build(classesInNamespace.Select(cls => cls.FullName.AlphanumericOnly())));
        }

        diagram.AddRelations(UmlSquaredLayout.Build(namespaces.Select(ns => ns.AlphanumericOnly())));

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        diagram.WriteToFile(outputPath, writeOptons);
    }
}