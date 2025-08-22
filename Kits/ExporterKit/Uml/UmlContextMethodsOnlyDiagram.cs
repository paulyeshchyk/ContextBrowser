using ContextBrowserKit.Extensions;
using ContextKit.Model;
using ContextKit.Model.Service;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, links, build
// pattern: Builder
public static class UmlContextMethodsOnlyDiagram
{
    // context: build, uml, links
    public static void Build(List<ContextInfo> elements, string outputPath, DiagramBuilderOptions options)
    {
        var methods = elements
                .Where(e => e.ElementType == ContextInfoElementType.method)
                .ToList();

        var diagramId = $"methods_only_{outputPath}".AlphanumericOnly();
        var diagram = new UmlDiagramClasses(options, diagramId: diagramId);

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
    }

    private static void AddTransitionState(UmlDiagramClasses diagram, ContextInfo method, ContextInfo callee)
    {
        var state1 = new UmlState(method.Name ?? "<unknown method>", null);
        var state2 = new UmlState(callee?.Name ?? "<unknown callee>", null);
        var arrow = new UmlArrow();
        var transitionState = new UmlTransitionState(state1, state2, arrow);
        diagram.Add(transitionState);
    }
}