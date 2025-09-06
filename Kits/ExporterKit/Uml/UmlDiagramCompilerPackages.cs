using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;
using ExporterKit.Uml.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerPackages : IUmlDiagramCompiler
{
    // context: build, uml
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options)
    {
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.packages.domains.puml");
        var diagramId = $"packages_{outputPath}".AlphanumericOnly();

        var diagram = new UmlClassDiagram(options, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");

        var elements = contextInfoDataset.GetAll();

        var classes = elements.Where(e => e.ElementType == ContextInfoElementType.@class).ToList();
        var methods = elements.Where(e => e.ElementType == ContextInfoElementType.method).ToList();

        var grouped = classes.GroupBy(c => c.Namespace);

        foreach (var nsGroup in grouped)
        {
            AddPackage(diagram, methods, nsGroup);
        }

        diagram.WriteToFile(outputPath);

        return new Dictionary<string, bool>();
    }

    private static void AddPackage(UmlClassDiagram diagram, List<ContextInfo> methods, IGrouping<string, ContextInfo> nsGroup)
    {
        var package = new UmlPackage(nsGroup.Key, alias: nsGroup.Key.AlphanumericOnly(), url: UmlUrlBuilder.BuildNamespaceUrl(nsGroup.Key));

        foreach (var cls in nsGroup)
        {
            AddComponentGroup(methods, package, cls);
        }

        diagram.Add(package);
        diagram.AddRelations(UmlSquaredLayout.Build(methods.Select(m => m.FullName)));
    }

    private static void AddComponentGroup(List<ContextInfo> methods, UmlPackage package, ContextInfo cls)
    {
        var stereotype = cls.Contexts.Any()
            ? string.Join(", ", cls.Contexts.OrderBy(c => c))
            : "NoContext";

        var compGroup = new UmlComponentGroup(CleanName(cls.Name), stereotype);

        var methodsInClass = methods.Where(m => m.ClassOwner?.Name == cls.Name);
        foreach (var method in methodsInClass)
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
        if (string.IsNullOrEmpty(rawName))
            return rawName;

        return Regex.Replace(rawName, @"<(.+?)>", "[$1]")
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
    }
}