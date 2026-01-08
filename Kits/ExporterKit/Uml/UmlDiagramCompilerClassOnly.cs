using System;
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
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

// context: uml, build
public class UmlDiagramCompilerClassOnly : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlDiagramCompilerClassOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;

    }

    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassOnly");

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var contextInfoDataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var classesOnly = contextInfoDataset.GetAll().Where(c => (c.ElementType.IsEntityDefinition()));

        var tasks = classesOnly.Select(async context => await BuildAsync(contextInfo: context,
                                                                       exportOptions: exportOptions,
                                                                             options: diagramBuilderOptions,
                                                                             methods: GetMethods(contextInfoDataset),
                                                                          properties: GetProperties(contextInfoDataset),
                                                                   cancellationToken: cancellationToken)
        );
        await Task.WhenAll(tasks);
        return new Dictionary<ILabeledValue, bool>();
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetProperties(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll()
            .Where(c => c.ElementType == ContextInfoElementType.property && c.ClassOwner?.FullName == contextInfo.FullName);
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetMethods(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll()
            .Where(c => c.ElementType == ContextInfoElementType.method && c.ClassOwner?.FullName == contextInfo.FullName);
    }

    //context: uml, build, heatmap, directory
    internal async Task BuildAsync(IContextInfo contextInfo, ExportOptions exportOptions, DiagramBuilderOptions options, Func<IContextInfo, IEnumerable<IContextInfo>> methods, Func<IContextInfo, IEnumerable<IContextInfo>> properties, CancellationToken cancellationToken)
    {
        var fullName = $"{contextInfo.FullName.AlphanumericOnly()}";
        var classNameWithNameSpace = $"{contextInfo.Namespace}.{contextInfo.ShortName}";
        var pumlFileName = _namingProcessor.ClassOnlyPumlFilename(classNameWithNameSpace);
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, pumlFileName);

        var diagramId = _namingProcessor.ClassOnlyDiagramId(fullName);
        var diagramTitle = _namingProcessor.ClassOnlyDiagramTitle(contextInfo.Name);

        var diagram = new UmlDiagramClass(options, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var packageUrl = _umlUrlBuilder.BuildNamespaceUrl(contextInfo.Namespace);
        var package = new UmlPackage(contextInfo.Namespace, alias: contextInfo.Namespace.AlphanumericOnly(), url: packageUrl);
        diagram.Add(package);

        var entityType = contextInfo.ElementType.ConvertToUmlEntityType();
        var umlClass = new UmlEntity(entityType, contextInfo.Name, contextInfo.Name.AlphanumericOnly(), url: null);
        package.Add(umlClass);

        var classMethods = methods(contextInfo);
        foreach (var element in classMethods)
        {
            var umlMethod = new UmlMethod(element.Name + "()", visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }

        var classProperties = properties(contextInfo);
        foreach (var element in classProperties)
        {
            var umlMethod = new UmlMethod(element.Name + "()", visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        await diagram.WriteToFileAsync(fileName, writeOptons, cancellationToken);
    }
}
