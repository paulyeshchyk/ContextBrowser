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

        var diagram = new UmlDiagramClasses(options);

        foreach (var method in methods)
            diagram.Add(new UmlComponent(method.Name));

        foreach (var method in methods)
        {
            var references = ContextInfoService.GetReferencesSortedByInvocation(method);
            foreach (var callee in references)
            {
                if (methods.Any(m => m.Name == callee.Name))
                    diagram.Add(new UmlTransitionState(new UmlState(method.Name ?? "<unknown method>", null), new UmlState(callee?.Name ?? "<unknown callee>", null), new UmlArrow()));
            }
        }

        diagram.WriteToFile(outputPath);
    }
}