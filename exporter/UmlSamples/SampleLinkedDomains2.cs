using ContextBrowser.model;
using System.Text.RegularExpressions;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
public static class SampleLinkedDomains2
{
    // context: build, uml
    public static void GenerateUml(List<ContextInfo> elements, string outputPath)
    {
        var diagram = new UmlDiagram();

        var classes = elements.Where(e => e.ElementType == ContextInfoElementType.@class).ToList();
        var methods = elements.Where(e => e.ElementType == ContextInfoElementType.method).ToList();

        var grouped = classes.GroupBy(c => c.Namespace ?? "Global");

        foreach (var nsGroup in grouped)
        {
            var package = new UmlPackage(nsGroup.Key);

            foreach (var cls in nsGroup)
            {
                var stereotype = cls.Contexts.Any()
                    ? string.Join(", ", cls.Contexts.OrderBy(c => c))
                    : "NoContext";

                var compGroup = new UmlComponentGroup(CleanName(cls.Name), stereotype);

                var methodsInClass = methods.Where(m => m.ClassOwner == cls.Name);
                foreach (var method in methodsInClass)
                {
                    var methodStereotype = string.Join(", ", method.Contexts.Distinct().OrderBy(x => x));
                    var methodBox = new UmlMethodBox(CleanName(method.Name), methodStereotype);
                    compGroup.Add(methodBox);
                }

                package.Add(compGroup);
            }

            diagram.Add(package);
        }

        diagram.WriteToFile(outputPath);
    }

    // context: uml, build
    private static string? CleanName(string? rawName)
    {
        if (string.IsNullOrEmpty(rawName))
            return rawName;

        return Regex.Replace(rawName, @"<(.+?)>", "[$1]")
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
    }
}
