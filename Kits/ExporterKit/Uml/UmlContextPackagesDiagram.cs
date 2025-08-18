using ContextBrowserKit.Extensions;
using ContextKit.Model;
using System.Text.RegularExpressions;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextPackagesDiagram
{
    // context: build, uml
    public static void Build(List<ContextInfo> elements, string outputPath, DiagramBuilderOptions options)
    {
        var diagramId = $"packages_{outputPath}".AlphanumericOnly();
        var diagram = new UmlDiagramClasses(options, diagramId: diagramId);

        var classes = elements.Where(e => e.ElementType == ContextInfoElementType.@class).ToList();
        var methods = elements.Where(e => e.ElementType == ContextInfoElementType.method).ToList();

        var grouped = classes.GroupBy(c => c.Namespace);

        foreach(var nsGroup in grouped)
        {
            AddPackage(diagram, methods, nsGroup);
        }

        diagram.WriteToFile(outputPath);
    }

    private static void AddPackage(UmlDiagramClasses diagram, List<ContextInfo> methods, IGrouping<string, ContextInfo> nsGroup)
    {
        var package = new UmlPackage(nsGroup.Key);

        foreach(var cls in nsGroup)
        {
            AddComponentGroup(methods, package, cls);
        }

        diagram.Add(package);
    }

    private static void AddComponentGroup(List<ContextInfo> methods, UmlPackage package, ContextInfo cls)
    {
        var stereotype = cls.Contexts.Any()
            ? string.Join(", ", cls.Contexts.OrderBy(c => c))
            : "NoContext";

        var compGroup = new UmlComponentGroup(CleanName(cls.Name), stereotype);

        var methodsInClass = methods.Where(m => m.ClassOwner?.Name == cls.Name);
        foreach(var method in methodsInClass)
        {
            AddMethodBox(compGroup, method);
        }

        package.Add(compGroup);
    }

    private static void AddMethodBox(UmlComponentGroup compGroup, ContextInfo method)
    {
        var methodStereotype = string.Join(", ", method.Contexts.Distinct().OrderBy(x => x));
        var methodBox = new UmlMethodBox(CleanName(method.Name), methodStereotype);
        compGroup.Add(methodBox);
    }

    // context: uml, build
    private static string? CleanName(string? rawName)
    {
        if(string.IsNullOrEmpty(rawName))
            return rawName;

        return Regex.Replace(rawName, @"<(.+?)>", "[$1]")
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
    }
}