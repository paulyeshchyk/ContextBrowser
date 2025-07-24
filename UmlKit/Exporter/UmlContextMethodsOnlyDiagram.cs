using ContextBrowser.ContextKit.Model;
using ContextBrowser.UmlKit.Diagrams;
using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Exporter;

// context: uml, links, build
// pattern: Builder
public static class UmlContextMethodsOnlyDiagram
{
    // context: build, uml, links
    public static void Build(List<ContextInfo> elements, string outputPath)
    {
        var methods = elements
                .Where(e => e.ElementType == ContextInfoElementType.method)
                .ToList();

        var diagram = new UmlDiagram();

        foreach(var method in methods)
            diagram.Add(new UmlComponent(method.Name));

        foreach(var method in methods)
        {
            foreach(var callee in method.References)
            {
                if(methods.Any(m => m.Name == callee.Name))
                    diagram.Add(new UmlRelation(method.Name, callee?.Name ?? "<unknown>"));
            }
        }

        diagram.WriteToFile(outputPath);
    }
}