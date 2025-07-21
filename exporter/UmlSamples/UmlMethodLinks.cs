using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, links, build
public static class UmlMethodLinks
{
    // context: build, uml, links
    public static void GenerateMethodLinks(List<ContextInfo> elements, string outputPath)
    {
        var methods = elements
                .Where(e => e.ElementType == ContextInfoElementType.method)
                .ToList();

        var diagram = new UmlDiagram();

        foreach (var method in methods)
            diagram.Add(new UmlComponent(method.Name));

        foreach (var method in methods)
        {
            foreach (var callee in method.References)
            {
                if (methods.Any(m => m.Name == callee))
                    diagram.Add(new UmlRelation(method.Name, callee));
            }
        }

        diagram.WriteToFile(outputPath);
    }
}