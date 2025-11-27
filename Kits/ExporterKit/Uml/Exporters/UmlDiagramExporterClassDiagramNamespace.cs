using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using ExporterKit.Uml.Exporters;
using UmlKit.Builders.Url;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.Exporters;

public class UmlDiagramExporterClassDiagramNamespace
{
    //context: uml, build
    public static void Export(string nameSpace, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, IEnumerable<IContextInfo> classesList)
    {
        var maxLength = Math.Max(nameSpace.Length, classesList.Any() ? classesList.Max(ns => ns.Name.Length) : 0);

        var diagramId = $"namespace_only_{nameSpace.AlphanumericOnly()}";
        var diagramTitle = $"Package diagram -> {nameSpace}";

        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var packageUrl = UmlUrlBuilder.BuildNamespaceUrl(nameSpace);
        var package = new UmlPackage(nameSpace.PadRight(maxLength), alias: nameSpace.AlphanumericOnly(), url: packageUrl);
        diagram.Add(package);

        foreach (var contextInfo in classesList)
        {
            string? htmlUrl = UmlUrlBuilder.BuildClassUrl(contextInfo);
            var entityType = contextInfo.ElementType.ConvertToUmlEntityType();
            var umlClass = new UmlEntity(entityType, contextInfo.Name.PadRight(maxLength), contextInfo.Name.AlphanumericOnly(), url: htmlUrl);
            package.Add(umlClass);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"namespace_only_{nameSpace.AlphanumericOnly()}.puml");
        diagram.WriteToFile(fileName, writeOptons);
    }
}