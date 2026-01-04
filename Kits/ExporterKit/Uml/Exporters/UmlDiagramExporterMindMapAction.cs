using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using GraphKit.Walkers;
using TensorKit.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.Exporters;

public class UmlDiagramExporterMindMapAction
{
    public static void Export(IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, string action, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        var fileName = namingProcessor.MindmapActionPumlFilename(action);
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        var diagramId = namingProcessor.MindmapActionDiagramId(outputPath);

        var diagram = new UmlDiagramMindmap(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");
        diagram.AddStyle(UmlStyle.Builder("grey").BackgroundColor("#f0f0f0").LineColor("#e0e0e0").HyperlinkColor("#101010").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("green").BackgroundColor("#90de90").LineColor("#60d060").HyperlinkColor("#1d601d").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("selected").BackgroundColor("#ff9500").LineColor("#d47c00").HyperlinkColor("#1f1510").HyperlinkUnderlineThickness(0).Build());

        var node = new UmlNode(action, url: umlUrlBuilder.BuildActionUrl(action))
        {
            Stylename = "selected"
        };

        IEnumerable<UmlNode> parentsForAction = GetParentsForAction(action, dataset, namingProcessor, umlUrlBuilder, (item) => item.ElementType != ContextInfoElementType.method);
        node.Parents.AddRange(parentsForAction);

        IEnumerable<UmlNode> childrenAction = GetChildrenForAction(action, dataset, namingProcessor, umlUrlBuilder, (item) => item.ElementType != ContextInfoElementType.method);
        node.Children.AddRange(childrenAction);

        diagram.Add(node);

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        diagram.WriteToFile(outputPath, writeOptons);
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
    private static IEnumerable<UmlNode> GetChildrenForAction(string action, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, Func<ContextInfo, bool> filter)
    {
        var startNodes = dataset.GetAll().Where(e => e.Action == action).DistinctBy(e => e.Identifier);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => new HashSet<ContextInfo>(x.References.Union(x.Owns)),
              createNode: UmlNodeTraverseBuilder.BuildMindNode,
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
    private static IEnumerable<UmlNode> GetParentsForAction(string action, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, Func<ContextInfo, bool> filter)
    {
        var startNodes = dataset.GetAll().Where(e => e.Action == action).DistinctBy(e => e.Identifier);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => x.InvokedBy,
              createNode: UmlNodeTraverseBuilder.BuildMindNode,
               linkNodes: (child, parent) => child.Parents.Add(parent),
         namingProcessor: namingProcessor,
           umlUrlBuilder: umlUrlBuilder,
                  filter: filter);
    }
}
