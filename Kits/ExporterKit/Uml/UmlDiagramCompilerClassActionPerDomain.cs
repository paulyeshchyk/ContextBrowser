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
public class UmlDiagramCompilerClassActionPerDomain<TDataTensor> : IUmlDiagramCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;

    public UmlDiagramCompilerClassActionPerDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
    }

    public async Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassActionPerDomain");

        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        foreach (var element in dataset)
        {
            Build(element, exportOptions, diagramBuilderOptions);
        }
        return new Dictionary<object, bool> { };
    }

    //context: uml, build, heatmap, directory
    internal void Build(KeyValuePair<TDataTensor, List<ContextInfo>> cell, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var contextInfoKey = cell.Key;
        var contextInfoList = cell.Value.Distinct().ToList();

        var pumlClass = _namingProcessor.ClassActionDomainPumlFilename(contextInfoKey.Action, contextInfoKey.Domain);
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, pumlClass);

        var diagramId = _namingProcessor.ClassActionDomainDiagramId(contextInfoKey.Action, contextInfoKey.Domain).AlphanumericOnly();
        var diagramTitle = _namingProcessor.ClassActionDomainDiagramTitle(contextInfoKey.Action, contextInfoKey.Domain);

        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        // Группируем по Namespace, затем по ClassOwnerFullName
        var allElements = UmlClassDiagramDataMapper.Map(contextInfoList);

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(allElements, new() { UmlDiagramMaxNamelengthExtractorType.@namespace, UmlDiagramMaxNamelengthExtractorType.@entity, UmlDiagramMaxNamelengthExtractorType.@method });

        var namespaces = allElements.GroupBy(e => e.Namespace);

        foreach (var nsGroup in namespaces)
        {
            var namespaceUrl = UmlUrlBuilder.BuildNamespaceUrl(nsGroup.Key);
            var package = new UmlPackage(nsGroup.Key.PadRight(maxLength), alias: nsGroup.Key.AlphanumericOnly(), url: namespaceUrl);
            diagram.Add(package);

            var classes = nsGroup.GroupBy(e => e.ClassName);

            foreach (var classGroup in classes)
            {
                foreach (var cls in classGroup)
                {
                    var htmlUrl = _namingProcessor.ClassOnlyHtmlFilename(cls.FullName);
                    var umlClass = new UmlEntity(UmlEntityType.@class, classGroup.Key.PadRight(maxLength), classGroup.Key.AlphanumericOnly(), url: htmlUrl);
                    package.Add(umlClass);

                    foreach (var element in classGroup)
                    {
                        string? url = null;//UmlUrlBuilder.BuildUrl(element);
                        var umlMethod = new UmlMethod(element.ContextInfo.Name + "()".PadRight(maxLength), visibility: UmlMemberVisibility.@public, url: url);
                        umlClass.Add(umlMethod);
                    }
                }
            }
            diagram.AddRelations(package.Elements.OrderBy(e => e.Key).Select(e => e.Value));
        }
        diagram.AddRelations(UmlSquaredLayout.Build(namespaces.Select(g => g.Key.AlphanumericOnly())));

        var writeOptons = new UmlWriteOptions(alignMaxWidth: maxLength) { };

        diagram.WriteToFile(fileName, writeOptons);
    }
}

