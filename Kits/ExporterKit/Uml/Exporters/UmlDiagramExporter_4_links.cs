using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ExporterKit;
using ExporterKit.Uml;
using ExporterKit.Uml.Exporters;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.Exporters;

public class UmlDiagramExporter_4_links
{
    public static async Task ExportAsync(ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, HashSet<(string From, string To)> links, CancellationToken cancellationToken)
    {
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.4.links.puml");
        var diagramId = $"relation_{outputPath}".AlphanumericOnly();
        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            AddRelation(diagramBuilderOptions, diagram, from, to);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        await diagram.WriteToFileAsync(outputPath, writeOptons, cancellationToken).ConfigureAwait(false);
    }

    private static void AddRelation(DiagramBuilderOptions options, UmlDiagramClass diagram, string from, string to)
    {
        var arrow = new UmlArrow(flowType: options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync);
        var relation = new UmlRelation(from, to, arrow);
        diagram.Add(relation);
    }
}