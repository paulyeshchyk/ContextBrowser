using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.Uml;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public class UmlClassDiagramCompilerMethodPerActionDomain : IUmlDiagramCompiler
{
    //context: build, uml

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        foreach (var cell in contextInfoDataSet.ContextInfoData)
        {
            var (action, domain) = cell.Key;
            var blockLabel = $"{action}";

            var listOfClasses = cell.Value.Distinct();
            if (!listOfClasses.Any())
            {
                continue;
            }

            // Вызываем метод для обработки данных по action
            BuildClassDomain(listOfClasses, exportOptions, diagramBuilderOptions, domain, blockLabel);
        }

        foreach (var cell in contextInfoDataSet.ContextInfoData)
        {
            var (action, domain) = cell.Key;

            var blockLabel = $"{domain}";
            var listOfClasses = cell.Value.Distinct();
            if (!listOfClasses.Any())
            {
                continue;
            }

            BuildClassAction(listOfClasses, exportOptions, diagramBuilderOptions, action, blockLabel);
        }

        return new Dictionary<string, bool>();
    }

    private static void BuildClassDomain(IEnumerable<ContextInfo> listOfClasses, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, string domain, string blockLabel)
    {
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"class_domain_{domain}.puml");
        var actionDiagramId = $"class_domain_{domain}".AlphanumericOnly();

        BuildPackageDiagram(diagramBuilderOptions, blockLabel, listOfClasses, actionOutputPath, actionDiagramId);
    }

    private static void BuildClassAction(IEnumerable<ContextInfo> listOfClasses, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, string action, string blockLabel)
    {
        var actionOutputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"class_action_{action}.puml");
        var actionDiagramId = $"class_action_{action}".AlphanumericOnly();

        BuildPackageDiagram(diagramBuilderOptions, blockLabel, listOfClasses, actionOutputPath, actionDiagramId);
    }

    private static void BuildPackageDiagram(DiagramBuilderOptions diagramBuilderOptions, string blockLabel, IEnumerable<ContextInfo> listOfClasses, string outputPath, string diagramId)
    {
        var diagram = new UmlClassDiagram(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");

        AddPackage(diagram, blockLabel, listOfClasses);
        diagram.WriteToFile(outputPath);
    }

    private static void AddPackage(UmlClassDiagram diagram, string blockLabel, IEnumerable<ContextKit.Model.ContextInfo> listOfClasses)
    {
        var package = new UmlPackage(blockLabel);

        foreach (var methodName in listOfClasses)
        {
            package.Add(new UmlComponent(methodName.FullName));
        }

        diagram.Add(package);
    }
}