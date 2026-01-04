using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
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
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlDiagramCompilerNamespaceOnly(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
    }

    //context: uml, build
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile Namespaces only");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var namespaces = GetNamespaces(dataset).ToList();
        foreach (var nameSpace in namespaces)
        {
            var classesList = GetClassesForNamespace(dataset)(nameSpace).ToList();

            UmlDiagramExporterClassDiagramNamespace.Export(diagramBuilderOptions: diagramBuilderOptions,
                                                                   exportOptions: exportOptions,
                                                                       nameSpace: nameSpace,
                                                                 namingProcessor: _namingProcessor,
                                                                     classesList: classesList,
                                                                   umlUrlBuilder: _umlUrlBuilder);
        }
        return new Dictionary<ILabeledValue, bool>();
    }

    private static Func<string, IEnumerable<IContextInfo>> GetClassesForNamespace(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return (nameSpace) => contextInfoDataSet.GetAll()
            .Where(c => c.ElementType.IsEntityDefinition())
            .Where(c => c.Namespace == nameSpace);
    }

    private IEnumerable<string> GetNamespaces(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return contextInfoDataSet.GetAll().Select(c => c.Namespace).Distinct();
    }
}
