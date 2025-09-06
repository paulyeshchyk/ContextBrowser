using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;
using ExporterKit.Uml.Model;
using LoggerKit;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerClassActionPerDomain : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public UmlDiagramCompilerClassActionPerDomain(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        foreach (var element in contextInfoDataset)
        {
            Build(element, exportOptions, diagramBuilderOptions);
        }
        return new Dictionary<string, bool> { };
    }

    //context: uml, build, heatmap, directory
    internal void Build(KeyValuePair<IContextKey, List<ContextInfo>> cell, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var contextInfoKey = cell.Key;
        var contextInfoList = cell.Value.Distinct().ToList();
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"class_{contextInfoKey.Action}_{contextInfoKey.Domain}.puml");

        var diagramId = $"class_{contextInfoKey.Action}_{contextInfoKey.Domain}".AlphanumericOnly();
        var diagramTitle = $"{contextInfoKey.Action.ToUpper()} -> {contextInfoKey.Domain}";

        var diagram = new UmlClassDiagram(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        // Группируем по Namespace, затем по ClassOwnerFullName
        var allElements = UmlClassDiagramDataMapper.Map(contextInfoList);

        var namespaces = allElements.GroupBy(e => e.Namespace);

        foreach (var nsGroup in namespaces)
        {
            var package = new UmlPackage(nsGroup.Key, alias: nsGroup.Key.AlphanumericOnly(), url: UmlUrlBuilder.BuildNamespaceUrl(nsGroup.Key));
            diagram.Add(package);

            var classes = nsGroup.GroupBy(e => e.ClassName);

            foreach (var classGroup in classes)
            {
                foreach (var cls in classGroup)
                {
                    string? htmlUrl = UmlUrlBuilder.BuildClassUrl(cls);
                    var umlClass = new UmlClass(classGroup.Key, classGroup.Key.AlphanumericOnly(), url: htmlUrl);
                    package.Add(umlClass);

                    foreach (var element in classGroup)
                    {
                        string? url = null;//UmlUrlBuilder.BuildUrl(element);
                        var umlMethod = new UmlMethod(element.ContextInfo.Name, Visibility: UmlMemberVisibility.@public, url: url);
                        umlClass.Add(umlMethod);
                    }
                }
            }
            diagram.AddRelations(package.Elements);
        }
        diagram.AddRelations(UmlSquaredLayout.Build(namespaces.Select(g => g.Key.AlphanumericOnly())));

        diagram.WriteToFile(fileName);
    }
}
