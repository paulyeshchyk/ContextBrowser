using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Service;
using ExporterKit.Uml;
using LoggerKit;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, links, build
// pattern: Builder
public class UmlDiagramCompilerClassMethodsList : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public UmlDiagramCompilerClassMethodsList(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
    }

    // context: build, uml, links
    public async Task<Dictionary<string, bool>> CompileAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "methodlinks.puml");

        var methods = dataset.GetAll()
                .Where(e => e.ElementType == ContextInfoElementType.method)
                .ToList();

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(methods, new() { UmlDiagramMaxNamelengthExtractorType.@method });

        var diagramId = $"methods_only_{outputPath}".AlphanumericOnly();
        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);

        foreach (var method in methods)
            diagram.Add(new UmlComponent(method.Name.PadRight(maxLength)));

        foreach (var method in methods)
        {
            var references = ContextInfoService.GetReferencesSortedByInvocation(method);
            foreach (var callee in references)
            {
                if (!methods.Any(m => m.Name == callee.Name))
                    continue;

                AddTransitionState(diagram, method, callee);
            }
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
        diagram.WriteToFile(outputPath, writeOptons);

        return new Dictionary<string, bool>();
    }

    private static void AddTransitionState(UmlDiagramClass diagram, ContextInfo method, ContextInfo callee)
    {
        var state1 = new UmlState(method.Name ?? "<unknown method>", null);
        var state2 = new UmlState(callee?.Name ?? "<unknown callee>", null);
        var arrow = new UmlArrow();
        var transitionState = new UmlTransitionState(state1, state2, arrow);
        diagram.Add(transitionState);
    }
}