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

        var classesOnly = contextInfoDataset.GetAll().Where(c => (c.ElementType.IsEntityDefinition() || c.MethodOwnedByItSelf == true));

        var tasks = classesOnly.Select(async context => await BuildAsync(contextInfo: context,
                                                                       exportOptions: exportOptions,
                                                                             options: diagramBuilderOptions,
                                                                        onGetMethods: GetMethods(contextInfoDataset),
                                                                     onGetProperties: GetProperties(contextInfoDataset),
                                                                   cancellationToken: cancellationToken)
        );
        await Task.WhenAll(tasks);
        return new Dictionary<ILabeledValue, bool>();
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetProperties(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.property && c.ClassOwner?.FullName == contextInfo.FullName).DistinctBy(e => e.FullName).OrderBy(e => e.ShortName);
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetMethods(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.method && c.ClassOwner?.FullName == contextInfo.FullName).DistinctBy(e => e.FullName).OrderBy(e => e.ShortName);
    }

    //context: uml, build, heatmap, directory
    internal async Task BuildAsync(IContextInfo contextInfo, ExportOptions exportOptions, DiagramBuilderOptions options, Func<IContextInfo, IEnumerable<IContextInfo>> onGetMethods, Func<IContextInfo, IEnumerable<IContextInfo>> onGetProperties, CancellationToken cancellationToken)
    {
        var fullName = $"{contextInfo.FullName.AlphanumericOnly()}";

        // грязный хак для получения информации о владельце
        var classownerInfo = contextInfo.MethodOwnedByItSelf
            ? (contextInfo.ClassOwner ?? contextInfo)
            : contextInfo;

        var classNameWithNameSpace = $"{classownerInfo.Namespace}.{classownerInfo.ShortName}";
        var pumlFileName = _namingProcessor.ClassOnlyPumlFilename(classNameWithNameSpace);
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, pumlFileName);

        var diagramId = _namingProcessor.ClassOnlyDiagramId(fullName);
        var diagramTitle = _namingProcessor.ClassOnlyDiagramTitle(contextInfo.Name);

        var diagram = new UmlDiagramClass(options, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var packageUrl = _umlUrlBuilder.BuildNamespaceUrl(classownerInfo.Namespace);
        var package = new UmlPackage(classownerInfo.Namespace, alias: classownerInfo.Namespace.AlphanumericOnly(), url: packageUrl);
        diagram.Add(package);

        var entityType = classownerInfo.ElementType.ConvertToUmlEntityType();
        var umlClass = new UmlEntity(entityType, classownerInfo.Name, classownerInfo.Name.AlphanumericOnly(), url: null);
        package.Add(umlClass);

        AddPropertiesAndMethods(umlClass, classownerInfo, onGetMethods, onGetProperties);

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        await diagram.WriteToFileAsync(fileName, writeOptons, cancellationToken);
    }

    internal static void AddPropertiesAndMethods(UmlEntity umlClass, IContextInfo classownerInfo, Func<IContextInfo, IEnumerable<IContextInfo>> onGetMethods, Func<IContextInfo, IEnumerable<IContextInfo>> onGetProperties)
    {
        var classMethods = onGetMethods(classownerInfo);
        foreach (var element in classMethods)
        {
            var umlMethod = new UmlMethod(element.Name + "()", visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }

        var classProperties = onGetProperties(classownerInfo);
        foreach (var element in classProperties)
        {
            var umlMethod = new UmlMethod(element.Name + "()", visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }
    }

}
