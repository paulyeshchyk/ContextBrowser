using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using ExporterKit.Uml.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerPackageMethodPerActionDomain : IUmlDiagramCompiler
{
    //context: build, uml

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        CompileDomainGroup(contextInfoDataset, exportOptions, diagramBuilderOptions);
        CompileNoDomainGroup(contextInfoDataset, exportOptions, diagramBuilderOptions);

        CompileActionGroup(contextInfoDataset, exportOptions, diagramBuilderOptions);
        CompileNoActionGroup(contextInfoDataset, exportOptions, diagramBuilderOptions);

        return new Dictionary<string, bool>();
    }

    private static void CompileActionGroup(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var actionContexts = contextInfoDataset
            .SelectMany(cell => cell.Value.Select(context => (action: cell.Key.Action, context: context)))
            .GroupBy(item => item.action)
            .Where(group => !string.IsNullOrWhiteSpace(group.Key));

        foreach (var group in actionContexts)
        {
            var action = group.Key;
            var contexts = group.Select(item => item.context).Distinct().ToList();

            if (contexts.Any())
            {
                BuildPackageAction(contexts, exportOptions, diagramBuilderOptions, action, "?");
            }
        }
    }

    private static void CompileNoActionGroup(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var actionContexts = contextInfoDataset
                .SelectMany(cell => cell.Value)
                .Where(context => string.IsNullOrWhiteSpace(context.Action))
                .ToList();

        BuildPackageAction(actionContexts, exportOptions, diagramBuilderOptions, "NoAction", "?");
    }

    private static void CompileDomainGroup(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var domainContexts = contextInfoDataset
            .SelectMany(cell => cell.Value)
            .GroupBy(context => context.Domains.FirstOrDefault())
            .Where(group => group.Key != null);

        foreach (var group in domainContexts)
        {
            var domain = group.Key; // The unique domain string
            var contexts = group.ToList(); // The list of all ContextInfo for that domain

            BuildPackageDomain(contexts, exportOptions, diagramBuilderOptions, domain, "?");
        }
    }

    private static void CompileNoDomainGroup(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var domainContexts = contextInfoDataset
            .SelectMany(cell => cell.Value)
            .Where(context => !context.Domains.Any())
            .ToList();

        if (domainContexts.Any())
        {
            // Вместо группировки - просто передаём список
            BuildPackageDomain(domainContexts, exportOptions, diagramBuilderOptions, "NoDomain", "?");
        }
    }

    private static void BuildPackageDomain(IEnumerable<ContextInfo> items, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, string domain, string blockLabel)
    {
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"class_domain_{domain}.puml");
        var actionDiagramId = $"class_domain_{domain}".AlphanumericOnly();

        BuildPackageDiagram(diagramBuilderOptions, blockLabel, items, actionOutputPath, actionDiagramId);
    }

    private static void BuildPackageAction(IEnumerable<ContextInfo> items, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, string action, string blockLabel)
    {
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"class_action_{action}.puml");
        var actionDiagramId = $"class_action_{action}".AlphanumericOnly();

        BuildPackageDiagram(diagramBuilderOptions, blockLabel, items, actionOutputPath, actionDiagramId);
    }

    private static void BuildPackageDiagram(DiagramBuilderOptions diagramBuilderOptions, string blockLabel, IEnumerable<ContextInfo> items, string outputPath, string diagramId)
    {
        // Создаем новую диаграмму
        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetAllowMixing();
        diagram.SetSeparator("none");

        var classesonly = items.Where(item => item.ElementType == ContextInfoElementType.@class).ToList();
        var methodsonly = items.Where(item => item.ElementType == ContextInfoElementType.@method).ToList();
        var propssonly = items.Where(item => item.ElementType == ContextInfoElementType.property).ToList();
        var propssonlyClassOwners = items.Where(item => item.ElementType == ContextInfoElementType.property).Select(i => i.ClassOwner).ToList();
        var methodsonlyClassOwners = items.Where(item => item.ElementType == ContextInfoElementType.@method).Select(i => i.ClassOwner).ToList();

        var classes = classesonly.Concat(methodsonlyClassOwners)
                                 .Concat(propssonlyClassOwners)
                                 .Cast<ContextInfo>().Distinct();

        var namespaces = classes.Select(c => c.Namespace).Where(ns => !string.IsNullOrWhiteSpace(ns)).Distinct().ToList();

        foreach (var ns in namespaces)
        {
            var umlPackage = new UmlPackage(ns, alias: ns.AlphanumericOnly(), url: UmlUrlBuilder.BuildNamespaceUrl(ns));

            var classesInNamespace = classes.Where(c => c.Namespace == ns).Distinct().ToList();

            foreach (var cls in classesInNamespace)
            {
                string? htmlUrl = UmlUrlBuilder.BuildClassUrl(cls);

                var entityType = ContextInfoExt.ConvertToUmlEntityType(cls.ElementType);
                var umlClass = new UmlEntity(entityType, cls.Name, cls.FullName.AlphanumericOnly(), url: htmlUrl);
                var propsList = propssonly.Where(p => p.ClassOwner?.FullName.Equals(cls?.FullName) ?? false).Distinct();
                foreach (var element in propsList)
                {
                    string? url = null;// UmlUrlBuilder.BuildUrl(element);
                    umlClass.Add(new UmlProperty(element.ShortName, visibility: UmlMemberVisibility.@public, url: url));
                }

                var methodList = methodsonly.Where(m => m.ClassOwner?.FullName.Equals(cls?.FullName) ?? false).Distinct();
                foreach (var element in methodList)
                {
                    string? url = null;//UmlUrlBuilder.BuildUrl(element);
                    umlClass.Add(new UmlMethod(element.ShortName, Visibility: UmlMemberVisibility.@public, url: url));
                }

                umlPackage.Add(umlClass);
            }

            diagram.Add(umlPackage);
            diagram.AddRelations(UmlSquaredLayout.Build(classesInNamespace.Select(cls => cls.FullName.AlphanumericOnly())));
        }

        diagram.AddRelations(UmlSquaredLayout.Build(namespaces.Select(ns => ns.AlphanumericOnly())));

        diagram.WriteToFile(outputPath);
    }
}