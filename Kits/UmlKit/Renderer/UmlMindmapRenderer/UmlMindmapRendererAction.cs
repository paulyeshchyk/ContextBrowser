using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using GraphKit.Walkers;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlMindmapRendererAction
{
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly IAppOptionsStore _optionsStore;

    public UmlMindmapRendererAction(IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, IAppOptionsStore optionsStore)
    {
        _datasetProvider = datasetProvider;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _optionsStore = optionsStore;
    }

    public async Task<UmlRendererResult<UmlDiagramMindmap>> RenderAsync(string action, CancellationToken cancellationToken)
    {
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var node = new UmlNode(action, url: _umlUrlBuilder.BuildActionUrl(action))
        {
            Stylename = "selected"
        };

        IEnumerable<UmlNode> parentsForAction = GetParentsForAction(action, dataset, _namingProcessor, _umlUrlBuilder, (item) => item.ElementType != ContextInfoElementType.method);
        node.Parents.AddRange(parentsForAction);

        IEnumerable<UmlNode> childrenAction = GetChildrenForAction(action, dataset, _namingProcessor, _umlUrlBuilder, (item) => item.ElementType != ContextInfoElementType.method);
        node.Children.AddRange(childrenAction);

        var diagram = new UmlDiagramMindmap(diagramBuilderOptions);
        diagram.Add(node);
        var result = new UmlRendererResult<UmlDiagramMindmap>(diagram, new UmlWriteOptions(alignMaxWidth: -1));
        return result;
    }

    /// <summary>
    /// Получает все "дочерние" домены, обходя граф по свойству References.
    /// </summary>
    /// <param name="action">Домен, для которого ищутся дети.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <param name="namingProcessor"></param>
    /// <param name="umlUrlBuilder"></param>
    /// <param name="filter"></param>
    /// <returns>Коллекция узлов UmlNode, представляющих дочерние домены.</returns>
    private static List<UmlNode> GetChildrenForAction(string action, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, Func<ContextInfo, bool> filter)
    {
        var startNodes = dataset.GetAll().Where(e => e.Action == action).DistinctBy(e => e.Identifier);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => new HashSet<ContextInfo>(x.References.Union(x.Owns)),
              createNode: PumlBuilderMindNode.BuildMindNode,
               linkNodes: (parent, child) => parent.Children.Add(child),
         namingProcessor: namingProcessor,
           umlUrlBuilder: umlUrlBuilder,
                  filter: filter);
    }

    /// <summary>
    /// Получает все "родительские" домены, обходя граф по свойству InvokedBy.
    /// </summary>
    /// <param name="action">Домен, для которого ищутся родители.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <param name="namingProcessor"></param>
    /// <param name="umlUrlBuilder"></param>
    /// <param name="filter"></param>
    /// <returns>Коллекция узлов UmlNode, представляющих родительские домены.</returns>
    private static List<UmlNode> GetParentsForAction(string action, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, Func<ContextInfo, bool> filter)
    {
        var startNodes = dataset.GetAll().Where(e => e.Action == action).DistinctBy(e => e.Identifier);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => x.InvokedBy,
              createNode: PumlBuilderMindNode.BuildMindNode,
               linkNodes: (child, parent) => child.Parents.Add(parent),
         namingProcessor: namingProcessor,
           umlUrlBuilder: umlUrlBuilder,
                  filter: filter);
    }
}
