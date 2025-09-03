using System.Collections.Generic;
using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public class UmlClassDiagramCompilerRelation : IUmlDiagramCompiler
{
    //context: build, uml, links
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options)
    {
        var linkGenerator = new ContextInfoDataLinkGenerator(contextClassifier, contextInfoDataSet.ContextsList);
        var links = linkGenerator.Generate();

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.4.links.puml");
        var diagramId = $"relation_{outputPath}".AlphanumericOnly();
        var diagram = new UmlClassDiagram(options, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            AddRelation(options, diagram, from, to);
        }

        diagram.WriteToFile(outputPath);
        return new Dictionary<string, bool>();
    }

    private static void AddRelation(DiagramBuilderOptions options, UmlClassDiagram diagram, string from, string to)
    {
        var arrow = new UmlArrow(flowType: options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync);
        var relation = new UmlRelation(from, to, arrow);
        diagram.Add(relation);
    }
}