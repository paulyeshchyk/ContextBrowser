using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using GraphKit.Walkers;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

public class UmlDiagramCompilerMindmap : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerMindmap(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    // context: build, uml
    public async Task<Dictionary<string, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "Compile Mindmap");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var contextClassifier = _optionsStore.GetOptions<IDomainPerActionContextTensorClassifier>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll();
        var distinctDomains = elements.SelectMany(e => e.Domains).Distinct();

        foreach (var domain in distinctDomains)
        {
            var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"mindmap_domain_{domain}.puml");
            var diagramId = $"mindmap_{outputPath}".AlphanumericOnly();

            var diagram = new UmlDiagramMindmap(diagramBuilderOptions, diagramId: diagramId);
            diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
            diagram.SetSeparator("none");
            diagram.AddStyle(UmlStyle.Builder("grey").BackgroundColor("#f0f0f0").LineColor("#e0e0e0").HyperlinkColor("#101010").HyperlinkUnderlineThickness(0).Build());
            diagram.AddStyle(UmlStyle.Builder("green").BackgroundColor("#90de90").LineColor("#60d060").HyperlinkColor("#1d601d").HyperlinkUnderlineThickness(0).Build());
            diagram.AddStyle(UmlStyle.Builder("selected").BackgroundColor("#ff9500").LineColor("#d47c00").HyperlinkColor("#1f1510").HyperlinkUnderlineThickness(0).Build());

            var node = new UmlNode(domain, alias: null, url: UmlUrlBuilder.BuildDomainUrl(domain));
            node.Stylename = "selected";

            IEnumerable<UmlNode> parentsForDomain = GetParentsForDomain(domain, dataset);
            node.Parents.AddRange(parentsForDomain);

            IEnumerable<UmlNode> childrenDomain = GetChildrenForDomain(domain, dataset);
            node.Children.AddRange(childrenDomain);

            diagram.Add(node);

            var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
            diagram.WriteToFile(outputPath, writeOptons);
        }

        return new Dictionary<string, bool>();
    }

    /// <summary>
    /// Получает все "дочерние" домены, обходя граф по свойству References.
    /// </summary>
    /// <param name="domain">Домен, для которого ищутся дети.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <returns>Коллекция узлов UmlNode, представляющих дочерние домены.</returns>
    private static IEnumerable<UmlNode> GetChildrenForDomain(string domain, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset)
    {
        var startNodes = dataset.GetAll().Where(e => e.Domains.Contains(domain));

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => new HashSet<ContextInfo>(x.References.Union(x.Owns)),
              createNode: UmlNodeTraverseBuilder.BuildMindNode,
               linkNodes: (parent, child) => parent.Children.Add(child));
    }

    /// <summary>
    /// Получает все "родительские" домены, обходя граф по свойству InvokedBy.
    /// </summary>
    /// <param name="domain">Домен, для которого ищутся родители.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <returns>Коллекция узлов UmlNode, представляющих родительские домены.</returns>
    private static IEnumerable<UmlNode> GetParentsForDomain(string domain, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset)
    {
        var startNodes = dataset.GetAll().Where(e => e.Domains.Contains(domain));

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => x.InvokedBy,
              createNode: UmlNodeTraverseBuilder.BuildMindNode,
               linkNodes: (child, parent) => child.Parents.Add(parent));
    }
}

public static class UmlNodeTraverseBuilder
{
    public static UmlNode BuildMindNode(ContextInfo startNode)
    {
        // 1. Создание родительской ноды
        var ownerName = startNode.ClassOwner?.FullName ?? startNode.FullName;
        var resultNode = new UmlNode(ownerName, alias: null, url: UmlUrlBuilder.BuildClassUrl(ownerName));
        resultNode.Stylename = "grey";

        // Проверка, есть ли дочерние ноды
        if (startNode.Domains.Count == 0)
        {
            // Если доменов нет, то нода остаётся одна, без дочерних элементов
            return resultNode;
        }

        UmlNode? firstChild = null;
        UmlNode? previousChild = null;

        foreach (var domain in startNode.Domains)
        {
            var childNode = new UmlNode(domain, alias: null, UmlUrlBuilder.BuildDomainUrl(domain));
            childNode.Stylename = "green";
            resultNode.Children.Add(childNode);

            if (firstChild == null)
            {
                firstChild = childNode;
            }

            previousChild = childNode;
        }

        return resultNode;
    }
}