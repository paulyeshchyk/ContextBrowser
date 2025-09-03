using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextKit.Model.Matrix;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Compiler;

public class UmlClassDiagramCompiler
{
    private readonly DiagramBuilderOptions _options;
    private readonly string _outputPath;
    private readonly Func<string, string, string> _linkGenerator;

    public UmlClassDiagramCompiler(DiagramBuilderOptions options, string outputPath, Func<string, string, string> linkGenerator)
    {
        _options = options;
        _outputPath = outputPath;
        _linkGenerator = linkGenerator;
    }

    public void Build(IContextInfoData matrix)
    {
        var diagramId = $"actionPerDomain_{_outputPath}".AlphanumericOnly();

        var diagram = new UmlClassDiagram(_options, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var item in matrix)
        {
            var count = item.Value.Count;
            if (count == 0)
                continue;

            AddPackage(_linkGenerator, diagram, item.Key, count);
        }

        diagram.WriteToFile(_outputPath);
    }

    private static void AddPackage(Func<string, string, string> linkGenerator, UmlClassDiagram diagram, ContextInfoDataCell cell, int count)
    {
        var packageId = $"{cell.Action}_{cell.Domain}";
        var label = $"{packageId}\\nMethods: {count}";
        var url = linkGenerator(cell.Action, cell.Domain);

        var package = new UmlPackage(packageId);
        package.Add(new UmlComponent(label, url));
        diagram.Add(package);
    }
}
