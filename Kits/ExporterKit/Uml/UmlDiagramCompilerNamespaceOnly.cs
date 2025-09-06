using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
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
        var namespaces = GetNamespaces(contextInfoDataset);
        foreach (var nameSpace in namespaces)
        {
            Build(nameSpace: nameSpace,
              exportOptions: exportOptions,
      diagramBuilderOptions: diagramBuilderOptions,
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
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"namespace_only_{nameSpace.AlphanumericOnly()}.puml");

        var diagramId = $"namespace_only_{nameSpace.AlphanumericOnly()}";
        var diagramTitle = $"Package diagram -> {nameSpace}";

        var diagram = new UmlClassDiagram(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var package = new UmlPackage(nameSpace, alias: nameSpace.AlphanumericOnly(), url: UmlUrlBuilder.BuildNamespaceUrl(nameSpace));
        diagram.Add(package);

        var classesList = classes(nameSpace);
        foreach (var contextInfo in classesList)
        {
            var umlClass = new UmlClass(contextInfo.Name, contextInfo.Name.AlphanumericOnly(), url: null);
            package.Add(umlClass);

            //var classMethods = methods(contextInfo);
            //foreach (var element in classMethods)
            //{
            //    var umlMethod = new UmlMethod(element.Name, Visibility: UmlMemberVisibility.@public, url: null);
            //    umlClass.Add(umlMethod);
            //}

            //var classProperties = properties(contextInfo);
            //foreach (var element in classProperties)
            //{
            //    var umlMethod = new UmlMethod(element.Name, Visibility: UmlMemberVisibility.@public, url: null);
            //    umlClass.Add(umlMethod);
            //}
        }
        diagram.WriteToFile(fileName);
    }
}
