using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.Exporters;

public class UmlDiagramExporterClassDiagramNamespace
{
    //context: uml, build
    public static void Export(string nameSpace, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, List<IContextInfo> classesList)
    {
        var maxLength = Math.Max(nameSpace.Length, classesList.Any() ? classesList.Max(ns => ns.Name.Length) : 0);

        var diagramId = namingProcessor.NamespaceOnlyDiagramId(nameSpace);
        var diagramTitle = string.Format("Package diagram -> {0}", nameSpace);

        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        var packageUrl = umlUrlBuilder.BuildNamespaceUrl(nameSpace);
        var package = new UmlPackage(nameSpace.PadRight(maxLength), alias: nameSpace.AlphanumericOnly(), url: packageUrl);
        diagram.Add(package);

        foreach (var contextInfo in classesList)
        {
            var classNameWithNameSpace = $"{contextInfo.Namespace}.{contextInfo.ShortName}";

            var htmlUrl = namingProcessor.ClassOnlyHtmlFilename(classNameWithNameSpace);
            var entityType = contextInfo.ElementType.ConvertToUmlEntityType();
            var umlClass = new UmlEntity(entityType, contextInfo.Name.PadRight(maxLength), contextInfo.Name.AlphanumericOnly(), url: htmlUrl);
            package.Add(umlClass);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, namingProcessor.NamespaceOnlyPumlFilename(nameSpace));

        diagram.WriteToFile(fileName, writeOptons);
    }
}