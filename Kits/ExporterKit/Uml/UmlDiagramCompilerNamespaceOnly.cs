using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using ExporterKit.Uml.Exporters;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.Uml;

// context: uml, build
public class UmlDiagramCompilerNamespaceOnly<TDataTensor> : IUmlDiagramCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;

    public UmlDiagramCompilerNamespaceOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;

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
            var classesList = GetClassesForNamespace(dataset)(nameSpace);
            var methodLists = GetMethods(dataset);
            var properties = GetProperties(dataset);

            UmlDiagramExporterClassDiagramNamespace.Export(diagramBuilderOptions: diagramBuilderOptions,
                                                                   exportOptions: exportOptions,
                                                                       nameSpace: nameSpace,
                                                                 namingProcessor: _namingProcessor,
                                                                     classesList: classesList);
        }
        return new Dictionary<object, bool>();
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
