using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Uml;
using ExporterKit.Uml.Exporters;
using GraphKit.Walkers;
using TensorKit.Model;
using UmlKit.Builders.Url;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.Exporters;

public class UmlDiagramExporterMindMapDomain
{
    public static void Export(IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, string domain)
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