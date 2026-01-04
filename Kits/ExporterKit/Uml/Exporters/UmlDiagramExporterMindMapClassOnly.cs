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

public class UmlDiagramExporterMindMapClassOnly
{
    public static void Export(IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, ContextInfo classContextInfo, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        var fileName = namingProcessor.MindmapClassOnlyPumlFilename(classContextInfo.Name);
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        var diagramId = namingProcessor.MindmapActionDiagramId(outputPath);

        var diagram = new UmlDiagramMindmap(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");
        diagram.AddStyle(UmlStyle.Builder("grey").BackgroundColor("#f0f0f0").LineColor("#e0e0e0").HyperlinkColor("#101010").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("green").BackgroundColor("#90de90").LineColor("#60d060").HyperlinkColor("#1d601d").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("selected").BackgroundColor("#ff9500").LineColor("#d47c00").HyperlinkColor("#1f1510").HyperlinkUnderlineThickness(0).Build());

        var node = new UmlNode(classContextInfo.Name, url: umlUrlBuilder.BuildActionUrl(classContextInfo.Name))
        {
            Stylename = "selected"
        };

        IEnumerable<UmlNode> classMethods = GetClassMethods(classContextInfo, dataset, namingProcessor, umlUrlBuilder, (item) => item.ElementType == ContextInfoElementType.method);
        node.Children.AddRange(classMethods);

        diagram.Add(node);

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        diagram.WriteToFile(outputPath, writeOptons);
    }

    /// <summary>
    /// Получает все "родительские" домены, обходя граф по свойству InvokedBy.
    /// </summary>
    /// <param name="classContextInfo">Bнфо о классе.</param>
    /// <param name="dataset">Набор данных о контексте.</param>
    /// <param name="namingProcessor"></param>
    /// <param name="umlUrlBuilder"></param>
    /// <param name="filter"></param>
    /// <returns>Коллекция узлов UmlNode, представляющих родительские домены.</returns>
    private static IEnumerable<UmlNode> GetClassMethods(ContextInfo classContextInfo, IContextInfoDataset<ContextInfo, DomainPerActionTensor> dataset, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, Func<ContextInfo, bool> filter)
    {
        var startNodes = dataset.GetAll().Where(e => e.ElementType == ContextInfoElementType.method && classContextInfo.Equals(e.ClassOwner)).DistinctBy(e => e.Identifier);

        return DfsWalker_Traversal.Run(
              startItems: startNodes,
            nextSelector: x => x.InvokedBy,
              createNode: UmlNodeTraverseBuilder.BuildMindNode,
               linkNodes: (parent, child) => parent.Children.Add(child),
         namingProcessor: namingProcessor,
           umlUrlBuilder: umlUrlBuilder,
                  filter: filter);
    }
}