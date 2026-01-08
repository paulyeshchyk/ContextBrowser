using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.Model;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerPackages : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlDiagramCompilerPackages(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, IUmlUrlBuilder umlUrlBuilder)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _umlUrlBuilder = umlUrlBuilder;
    }

    // context: build, uml
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile Packages");

        var contextInfoDataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.packages.domains.puml");
        var diagramId = $"packages_{outputPath}".AlphanumericOnly();

        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");

        var elements = contextInfoDataset.GetAll().ToList();
        // int maxNameLength = UmlDiagramMaxNamelengthExtractor.Extract(elements,
        // [
        //     UmlDiagramMaxNamelengthExtractorType.@method,
        //     UmlDiagramMaxNamelengthExtractorType.@namespace,
        //     UmlDiagramMaxNamelengthExtractorType.entity,
        //     UmlDiagramMaxNamelengthExtractorType.property
        // ]);

        var classes = elements.Where(e => e.ElementType == ContextInfoElementType.@class).ToList();
        var methods = elements.Where(e => e.ElementType == ContextInfoElementType.method).ToList();

        var grouped = classes.GroupBy(c => c.Namespace);

        foreach (var nsGroup in grouped)
        {
            AddPackage(_umlUrlBuilder, diagram, methods, nsGroup);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        diagram.WriteToFile(outputPath, writeOptons);

        return new Dictionary<ILabeledValue, bool>();
    }

    private static void AddPackage(IUmlUrlBuilder umlUrlBuilder, UmlDiagramClass diagram, List<ContextInfo> methods, IGrouping<string, ContextInfo> namespaceContexts)
    {
        var packageUrl = umlUrlBuilder.BuildNamespaceUrl(namespaceContexts.Key);
        var package = new UmlPackage(namespaceContexts.Key, alias: namespaceContexts.Key.AlphanumericOnly(), url: packageUrl);

        foreach (var contextInfo in namespaceContexts)
        {
            AddComponentGroup(methods, package, contextInfo);
        }

        diagram.Add(package);
        diagram.AddRelations(UmlSquaredLayout.Build(methods.Select(m => m.FullName)));
    }

    private static void AddComponentGroup(List<ContextInfo> methods, UmlPackage package, ContextInfo cls)
    {
        var stereotype = cls.Contexts.Count != 0
            ? string.Join(", ", cls.Contexts.OrderBy(c => c))
            : "NoContext";

        var compGroup = new UmlComponentGroup(CleanName(cls.Name), stereotype);

        var methodsInClass = methods.Where(m => m.ClassOwner?.Name == cls.Name);
        foreach (var method in methodsInClass)
        {
            AddMethodBox(compGroup, method);
        }

        package.Add(compGroup);
    }

    private static void AddMethodBox(UmlComponentGroup compGroup, ContextInfo method)
    {
        var methodStereotype = string.Join(", ", method.Contexts.Distinct().OrderBy(x => x));
        var methodBox = new UmlMethodBox(CleanName(method.Name), methodStereotype);
        compGroup.Add(methodBox);
    }

    // context: uml, build
    private static string? CleanName(string? rawName)
    {
        if (string.IsNullOrEmpty(rawName))
            return rawName;

        return Regex.Replace(rawName, @"<(.+?)>", "[$1]")
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
    }
}
