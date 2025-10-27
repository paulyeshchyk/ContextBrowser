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
using GraphKit.Walkers;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders.Url;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

public class UmlDiagramCompilerMindmapAction : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerMindmapAction(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    // context: build, uml
    public async Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "Compile Mindmap Action");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll();
        var distinctAction = elements.Select(e => e.Action).Distinct().Where(s=>!string.IsNullOrWhiteSpace(s)).Select(s=>s!).Select(s=>s.Split(';')).SelectMany(s=>s).Distinct();

        foreach (var action in distinctAction)
        {
            var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"mindmap_action_{action}.puml");
            var diagramId = $"mindmap_{outputPath}".AlphanumericOnly();

            var diagram = new UmlDiagramMindmap(diagramBuilderOptions, diagramId: diagramId);
            diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
            diagram.SetSeparator("none");
            diagram.AddStyle(UmlStyle.Builder("grey").BackgroundColor("#f0f0f0").LineColor("#e0e0e0").HyperlinkColor("#101010").HyperlinkUnderlineThickness(0).Build());
            diagram.AddStyle(UmlStyle.Builder("green").BackgroundColor("#90de90").LineColor("#60d060").HyperlinkColor("#1d601d").HyperlinkUnderlineThickness(0).Build());
            diagram.AddStyle(UmlStyle.Builder("selected").BackgroundColor("#ff9500").LineColor("#d47c00").HyperlinkColor("#1f1510").HyperlinkUnderlineThickness(0).Build());

            var node = new UmlNode(action, alias: null, url: UmlUrlBuilder.BuildActionUrl(action));
            node.Stylename = "selected";

            IEnumerable<UmlNode> parentsForAction = GetParentsForAction(action, dataset);
            node.Parents.AddRange(parentsForAction);

            IEnumerable<UmlNode> childrenAction = GetChildrenForAction(action, dataset);
            node.Children.AddRange(childrenAction);

            diagram.Add(node);

            var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
            diagram.WriteToFile(outputPath, writeOptons);
        }

        return new Dictionary<object, bool>();
    }

    /// <summary>
    /// Получает все "дочерние" домены, обходя граф по свойству References.
    /// </summary>
    /// <param name="action">Домен, для которого ищутся дети.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <returns>Коллекция узлов UmlNode, представляющих дочерние домены.</returns>
    private static IEnumerable<UmlNode> GetChildrenForAction(string action, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset)
    {
        var startNodes = dataset.GetAll().Where(e => e.Action == action);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => new HashSet<ContextInfo>(x.References.Union(x.Owns)),
              createNode: UmlNodeTraverseBuilder.BuildMindNode,
               linkNodes: (parent, child) => parent.Children.Add(child));
    }

    /// <summary>
    /// Получает все "родительские" домены, обходя граф по свойству InvokedBy.
    /// </summary>
    /// <param name="action">Домен, для которого ищутся родители.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <returns>Коллекция узлов UmlNode, представляющих родительские домены.</returns>
    private static IEnumerable<UmlNode> GetParentsForAction(string action, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset)
    {
        var startNodes = dataset.GetAll().Where(e => e.Action == action);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => x.InvokedBy,
              createNode: UmlNodeTraverseBuilder.BuildMindNode,
               linkNodes: (child, parent) => child.Parents.Add(parent));
    }
}
