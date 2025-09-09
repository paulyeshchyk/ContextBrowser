using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using ExporterKit.Uml.Model;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
public class UmlDiagramCompilerClassOnly : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public UmlDiagramCompilerClassOnly(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var classesOnly = contextInfoDataset.GetAll()
            .Where(c => (c.ElementType == ContextInfoElementType.@class) || (c.ElementType == ContextInfoElementType.@struct) || (c.ElementType == ContextInfoElementType.record));

        foreach (var context in classesOnly)
        {
            Build(contextInfo: context,
                exportOptions: exportOptions,
                      options: diagramBuilderOptions,
                      methods: GetMethods(contextInfoDataset),
                   properties: GetProperties(contextInfoDataset));
        }
        return new Dictionary<string, bool> { };
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetProperties(IContextInfoDataset contextInfoDataSet)
    {
        return(contextInfo) => contextInfoDataSet.GetAll()
            .Where(c => c.ElementType == ContextInfoElementType.property && c.ClassOwner?.FullName == contextInfo.FullName);
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetMethods(IContextInfoDataset contextInfoDataSet)
    {
        return(contextInfo) => contextInfoDataSet.GetAll()
            .Where(c => c.ElementType == ContextInfoElementType.method && c.ClassOwner?.FullName == contextInfo.FullName);
    }

    //context: uml, build, heatmap, directory
    internal void Build(IContextInfo contextInfo, ExportOptions exportOptions, DiagramBuilderOptions options, Func<IContextInfo, IEnumerable<IContextInfo>> methods, Func<IContextInfo, IEnumerable<IContextInfo>> properties)
    {
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"class_only_{contextInfo.FullName.AlphanumericOnly()}.puml");

        var diagramId = $"class_only_{contextInfo.FullName.AlphanumericOnly()}";
        var diagramTitle = $"Class diagram -> {contextInfo.Name}";

        var diagram = new UmlDiagramClass(options, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var packageUrl = UmlUrlBuilder.BuildNamespaceUrl(contextInfo.Namespace);
        var package = new UmlPackage(contextInfo.Namespace, alias: contextInfo.Namespace.AlphanumericOnly(), url: packageUrl);
        diagram.Add(package);

        var entityType = ContextInfoExt.ConvertToUmlEntityType(contextInfo.ElementType);
        var umlClass = new UmlEntity(entityType, contextInfo.Name, contextInfo.Name.AlphanumericOnly(), url: null);
        package.Add(umlClass);

        var classMethods = methods(contextInfo);
        foreach (var element in classMethods)
        {
            var umlMethod = new UmlMethod(element.Name + "()", Visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }

        var classProperties = properties(contextInfo);
        foreach (var element in classProperties)
        {
            var umlMethod = new UmlMethod(element.Name + "()", Visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
        diagram.WriteToFile(fileName, writeOptons);
    }
}
