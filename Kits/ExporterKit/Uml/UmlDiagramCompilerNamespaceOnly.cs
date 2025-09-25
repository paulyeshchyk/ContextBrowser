using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using ExporterKit.Infrastucture;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
public class UmlDiagramCompilerNamespaceOnly<TDataTensor> : IUmlDiagramCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerNamespaceOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    //context: uml, build
    public async Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile Namespaces only");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var namespaces = GetNamespaces(dataset).ToList();
        foreach (var nameSpace in namespaces)
        {
            Build(diagramBuilderOptions: diagramBuilderOptions,
                          exportOptions: exportOptions,
                              nameSpace: nameSpace,
                                classes: GetClassesForNamespace(dataset),
                                methods: GetMethods(dataset),
                             properties: GetProperties(dataset));
        }
        return new Dictionary<object, bool> { };
    }

    //context: uml, build
    internal void Build(string nameSpace, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, Func<string, IEnumerable<IContextInfo>> classes, Func<IContextInfo, IEnumerable<IContextInfo>> methods, Func<IContextInfo, IEnumerable<IContextInfo>> properties)
    {
        var classesList = classes(nameSpace);
        var maxLength = Math.Max(nameSpace.Length, classesList.Any() ? classesList.Max(ns => ns.Name.Length) : 0);

        var diagramId = $"namespace_only_{nameSpace.AlphanumericOnly()}";
        var diagramTitle = $"Package diagram -> {nameSpace}";

        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var packageUrl = UmlUrlBuilder.BuildNamespaceUrl(nameSpace);
        var package = new UmlPackage(nameSpace.PadRight(maxLength), alias: nameSpace.AlphanumericOnly(), url: packageUrl);
        diagram.Add(package);

        foreach (var contextInfo in classesList)
        {
            string? htmlUrl = UmlUrlBuilder.BuildClassUrl(contextInfo);
            var entityType = ContextInfoExt.ConvertToUmlEntityType(contextInfo.ElementType);
            var umlClass = new UmlEntity(entityType, contextInfo.Name.PadRight(maxLength), contextInfo.Name.AlphanumericOnly(), url: htmlUrl);
            package.Add(umlClass);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"namespace_only_{nameSpace.AlphanumericOnly()}.puml");
        diagram.WriteToFile(fileName, writeOptons);
    }

    private static Func<string, IEnumerable<IContextInfo>> GetClassesForNamespace(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return (nameSpace) => contextInfoDataSet.GetAll()
            .Where(c => (c.ElementType == ContextInfoElementType.@class) || (c.ElementType == ContextInfoElementType.@struct) || (c.ElementType == ContextInfoElementType.record))
            .Where(c => c.Namespace == nameSpace);
    }

    private IEnumerable<string> GetNamespaces(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return contextInfoDataSet.GetAll().Select(c => c.Namespace).Distinct();
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetProperties(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.property && c.ClassOwner?.FullName == contextInfo.FullName);
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetMethods(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.method && c.ClassOwner?.FullName == contextInfo.FullName);
    }
}
