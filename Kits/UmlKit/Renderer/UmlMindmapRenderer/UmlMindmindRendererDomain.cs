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
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlMindmapRendererDomain
{
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly IAppOptionsStore _optionsStore;

    public UmlMindmapRendererDomain(IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, IAppOptionsStore optionsStore)
    {
        _datasetProvider = datasetProvider;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _optionsStore = optionsStore;
    }

    public async Task<UmlRendererResult<UmlDiagramMindmap>> RenderAsync(string domain, CancellationToken cancellationToken)
    {
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();
        var diagram = new UmlDiagramMindmap(diagramBuilderOptions);

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var node = new UmlNode(domain, url: _umlUrlBuilder.BuildDomainUrl(domain))
        {
            Stylename = "selected"
        };

        IEnumerable<UmlNode> parentsForDomain = GetParentsForDomain(domain, dataset, _namingProcessor, _umlUrlBuilder, (item) => item.ElementType != ContextInfoElementType.method);
        node.Parents.AddRange(parentsForDomain);

        IEnumerable<UmlNode> childrenDomain = GetChildrenForDomain(domain, dataset, _namingProcessor, _umlUrlBuilder, (item) => item.ElementType != ContextInfoElementType.method);
        node.Children.AddRange(childrenDomain);

        diagram.Add(node);
        var result = new UmlRendererResult<UmlDiagramMindmap>(diagram, new UmlWriteOptions(alignMaxWidth: -1));
        return result;
    }

    /// <summary>
    /// Получает все "дочерние" домены, обходя граф по свойству References.
    /// </summary>
    /// <param name="domain">Домен, для которого ищутся дети.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <param name="namingProcessor"></param>
    /// <param name="umlUrlBuilder"></param>
    /// <param name="filter"></param>
    /// <returns>Коллекция узлов UmlNode, представляющих дочерние домены.</returns>
    private static IEnumerable<UmlNode> GetChildrenForDomain(string domain, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, Func<ContextInfo, bool> filter)
    {
        var startNodes = dataset.GetAll().Where(e => e.Domains.Contains(domain)).DistinctBy(e => e.Identifier);

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
    /// <param name="domain">Домен, для которого ищутся родители.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <param name="namingProcessor"></param>
    /// <param name="umlUrlBuilder"></param>
    /// <param name="filter"></param>
    /// <returns>Коллекция узлов UmlNode, представляющих родительские домены.</returns>
    private static IEnumerable<UmlNode> GetParentsForDomain(string domain, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, Func<ContextInfo, bool> filter)
    {
        var startNodes = dataset.GetAll().Where(e => e.Domains.Contains(domain)).DistinctBy(e => e.Identifier);

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
