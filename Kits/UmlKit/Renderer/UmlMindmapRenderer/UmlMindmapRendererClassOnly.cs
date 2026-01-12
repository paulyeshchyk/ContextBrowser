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

public class UmlMindmapRendererClassOnly
{
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly IAppOptionsStore _optionsStore;


    public UmlMindmapRendererClassOnly(IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, IAppOptionsStore optionsStore)
    {
        _datasetProvider = datasetProvider;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _optionsStore = optionsStore;
    }

    public async Task<UmlRendererResult<UmlDiagramMindmap>> RenderAsync(ContextInfo classContextInfo, string classNameWithNameSpace, CancellationToken cancellationToken)
    {
        var node = new UmlNode(classContextInfo.Name, url: _umlUrlBuilder.BuildActionUrl(classNameWithNameSpace))
        {
            Stylename = "selected"
        };

        IEnumerable<UmlNode> classMethods = await GetClassMethods(classContextInfo, _datasetProvider, _namingProcessor, _umlUrlBuilder, (item) => item.ElementType == ContextInfoElementType.method, cancellationToken);
        node.Children.AddRange(classMethods);

        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();
        var diagram = new UmlDiagramMindmap(diagramBuilderOptions);
        diagram.Add(node);
        return new UmlRendererResult<UmlDiagramMindmap>(diagram, new UmlWriteOptions(alignMaxWidth: -1));
    }


    /// <summary>
    /// Получает все "родительские" домены, обходя граф по свойству InvokedBy.
    /// </summary>
    /// <param name="classContextInfo">Bнфо о классе.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <param name="_namingProcessor"></param>
    /// <param name="_umlUrlBuilder"></param>
    /// <param name="filter"></param>
    /// <returns>Коллекция узлов UmlNode, представляющих родительские домены.</returns>
    private static async Task<IEnumerable<UmlNode>> GetClassMethods(ContextInfo classContextInfo, IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider, INamingProcessor _namingProcessor, IUmlUrlBuilder _umlUrlBuilder, Func<ContextInfo, bool> filter, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
        var startNodes = dataset.GetAll().Where(e => e.ElementType == ContextInfoElementType.method && classContextInfo.Equals(e.ClassOwner)).DistinctBy(e => e.Identifier);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => x.InvokedBy,
              createNode: PumlBuilderMindNode.BuildMindNode,
               linkNodes: (parent, child) => parent.Children.Add(child),
         namingProcessor: _namingProcessor,
           umlUrlBuilder: _umlUrlBuilder,
                  filter: filter);
    }
}
