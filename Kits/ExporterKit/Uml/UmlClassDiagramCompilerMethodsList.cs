using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Service;
using ExporterKit.Uml;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, links, build
// pattern: Builder
public class UmlClassDiagramCompilerMethodsList : IUmlDiagramCompiler
{
    // context: build, uml, links
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options)
    {
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "methodlinks.puml");

        var methods = contextInfoDataset.ContextsList
                .Where(e => e.ElementType == ContextInfoElementType.method)
                .ToList();

        var diagramId = $"methods_only_{outputPath}".AlphanumericOnly();
        var diagram = new UmlClassDiagram(options, diagramId: diagramId);

        foreach (var method in methods)
            diagram.Add(new UmlComponent(method.Name));

        foreach (var method in methods)
        {
            var references = ContextInfoService.GetReferencesSortedByInvocation(method);
            foreach (var callee in references)
            {
                if (!methods.Any(m => m.Name == callee.Name))
                    continue;

                AddTransitionState(diagram, method, callee);
            }
        }

        diagram.WriteToFile(outputPath);

        return new Dictionary<string, bool>();
    }

    private static void AddTransitionState(UmlClassDiagram diagram, ContextInfo method, ContextInfo callee)
    {
        var state1 = new UmlState(method.Name ?? "<unknown method>", null);
        var state2 = new UmlState(callee?.Name ?? "<unknown callee>", null);
        var arrow = new UmlArrow();
        var transitionState = new UmlTransitionState(state1, state2, arrow);
        diagram.Add(transitionState);
    }
}