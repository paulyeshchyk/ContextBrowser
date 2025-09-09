using System.Collections.Generic;
using System.Linq;
using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, links, build
// pattern: Builder
public class UmlDiagramCompilerClassRelation : IUmlDiagramCompiler
{
    //context: build, uml, links
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options)
    {
        var contextInfoList = contextInfoDataset.GetAll().ToList();
        var linkGenerator = new ContextInfoDataLinkGenerator(contextClassifier, contextInfoList);
        var links = linkGenerator.Generate();

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.4.links.puml");
        var diagramId = $"relation_{outputPath}".AlphanumericOnly();
        var diagram = new UmlDiagramClass(options, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            AddRelation(options, diagram, from, to);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
        diagram.WriteToFile(outputPath, writeOptons);
        return new Dictionary<string, bool>();
    }

    private static void AddRelation(DiagramBuilderOptions options, UmlDiagramClass diagram, string from, string to)
    {
        var arrow = new UmlArrow(flowType: options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync);
        var relation = new UmlRelation(from, to, arrow);
        diagram.Add(relation);
    }
}
