using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using ExporterKit.Uml.Model;
using LoggerKit;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

public class UmlDiagramCompilerNamespaceOnly : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public UmlDiagramCompilerNamespaceOnly(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var namespaces = GetNamespaces(contextInfoDataset).ToList();
        foreach (var nameSpace in namespaces)
        {
            Build(diagramBuilderOptions: diagramBuilderOptions,
                          exportOptions: exportOptions,
                              nameSpace: nameSpace,
                                classes: GetClassesForNamespace(contextInfoDataset),
                                methods: GetMethods(contextInfoDataset),
                             properties: GetProperties(contextInfoDataset));
        }
        return new Dictionary<string, bool> { };
    }

    private static Func<string, IEnumerable<IContextInfo>> GetClassesForNamespace(IContextInfoDataset contextInfoDataSet)
    {
        return (nameSpace) => contextInfoDataSet.GetAll()
            .Where(c => (c.ElementType == ContextInfoElementType.@class) || (c.ElementType == ContextInfoElementType.@struct) || (c.ElementType == ContextInfoElementType.record))
            .Where(c => c.Namespace == nameSpace);
    }

    private IEnumerable<string> GetNamespaces(IContextInfoDataset contextInfoDataSet)
    {
        return contextInfoDataSet.GetAll().Select(c => c.Namespace).Distinct();
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetProperties(IContextInfoDataset contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.property && c.ClassOwner?.FullName == contextInfo.FullName);
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetMethods(IContextInfoDataset contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.method && c.ClassOwner?.FullName == contextInfo.FullName);
    }

    //context: uml, build, heatmap, directory
    internal void Build(string nameSpace, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, Func<string, IEnumerable<IContextInfo>> classes, Func<IContextInfo, IEnumerable<IContextInfo>> methods, Func<IContextInfo, IEnumerable<IContextInfo>> properties)
    {

        var classesList = classes(nameSpace);
        var maxLength = Math.Max(nameSpace.Length, classesList.Any() ? classesList.Max(ns => ns.Name.Length) : 0);

        var diagramId = $"namespace_only_{nameSpace.AlphanumericOnly()}";
        var diagramTitle = $"Package diagram -> {nameSpace}";

        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var package = new UmlPackage(nameSpace.PadRight(maxLength), alias: nameSpace.AlphanumericOnly(), url: UmlUrlBuilder.BuildNamespaceUrl(nameSpace));
        diagram.Add(package);

        foreach (var contextInfo in classesList)
        {
            string? htmlUrl = UmlUrlBuilder.BuildClassUrl(contextInfo);
            var entityType = ContextInfoExt.ConvertToUmlEntityType(contextInfo.ElementType);
            var calculatedName = contextInfo.Name.PadRight(maxLength);
            var umlClass = new UmlEntity(entityType, calculatedName, contextInfo.Name.AlphanumericOnly(), url: htmlUrl);
            package.Add(umlClass);
        }

        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"namespace_only_{nameSpace.AlphanumericOnly()}.puml");
        diagram.WriteToFile(fileName, -1);
    }
}
