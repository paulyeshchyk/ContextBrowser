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
using ExporterKit.Uml.Builder;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.DiagramCompiler;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerClassActionPerDomain<TDataTensor> : IUmlDiagramCompiler
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlDiagramCompilerClassActionPerDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
    }

    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassActionPerDomain");

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
        var tasks = dataset.Select(async e => await BuildAsync(e, exportOptions, diagramBuilderOptions, cancellationToken).ConfigureAwait(false));
        await Task.WhenAll(tasks);
        return new Dictionary<ILabeledValue, bool>();
    }

    //context: uml, build, heatmap, directory
    internal async Task BuildAsync(KeyValuePair<TDataTensor, List<ContextInfo>> cell, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var contextInfoKey = cell.Key;
        var contextInfoList = cell.Value.Distinct().Where(c => (c.ElementType.IsEntityDefinition())).ToList();

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
        var allElements = UmlClassDiagramDataMapper.Map(contextInfoList).ToList();

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(allElements, new() { UmlDiagramMaxNamelengthExtractorType.@namespace, UmlDiagramMaxNamelengthExtractorType.@entity, UmlDiagramMaxNamelengthExtractorType.@method });

        var namespaces = allElements.GroupBy(e => e.Namespace).ToList();

        foreach (var nsGroup in namespaces)
        {
            var package = PumlBuilderHelper.BuildUmlPackage(maxLength, nsGroup, _umlUrlBuilder);

            var classes = nsGroup.GroupBy(e => e.ClassName);

            foreach (var classGroup in classes)
            {
                foreach (var cls in classGroup)
                {
                    var umlClass = PumlBuilderHelper.BuildUmlEntityClass(cls.ContextInfo, classGroup, maxLength, _namingProcessor);
                    package.Add(umlClass);
                }
            }
            diagram.Add(package);
            diagram.AddRelations(package.Elements.OrderBy(e => e.Key).Select(e => e.Value));
        }
        diagram.AddRelations(PumlBuilderSquaredLayout.Build(namespaces.Select(g => g.Key.AlphanumericOnly())));

        var writeOptons = new UmlWriteOptions(alignMaxWidth: maxLength);

        await diagram.WriteToFileAsync(fileName, writeOptons, cancellationToken).ConfigureAwait(false);
    }
}
