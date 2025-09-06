using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextKit.Model;
using UmlKit;
using UmlKit.Compiler;
using UmlKit.Compiler.CoCompiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Compiler.CoCompiler;

public class UmlDiagramCompilerClass
{
    private readonly DiagramBuilderOptions _options;
    private readonly string _outputPath;
    private readonly Func<string, string, string> _linkGenerator;

    public UmlDiagramCompilerClass(DiagramBuilderOptions options, string outputPath, Func<string, string, string> linkGenerator)
    {
        _options = options;
        _outputPath = outputPath;
        _linkGenerator = linkGenerator;
    }

    public void Build(IContextInfoDataset matrix)
    {
        var diagramId = $"actionPerDomain_{_outputPath}".AlphanumericOnly();

        var diagram = new UmlClassDiagram(_options, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        foreach (var item in matrix)
        {
            var count = item.Value.Count;
            if (count == 0)
                continue;

            AddPackage(_linkGenerator, diagram, item.Key, count);
        }

        diagram.WriteToFile(_outputPath);
    }

    private static void AddPackage(Func<string, string, string> linkGenerator, UmlClassDiagram diagram, IContextKey cell, int count)
    {
        var packageId = $"{cell.Action}_{cell.Domain}";
        var label = $"{packageId}\\nMethods: {count}";
        var url = linkGenerator(cell.Action, cell.Domain);

#warning no url defined
        var package = new UmlPackage(packageId, alias: packageId.AlphanumericOnly(), url: null);
        package.Add(new UmlComponent(label, url));
        diagram.Add(package);
    }
}
