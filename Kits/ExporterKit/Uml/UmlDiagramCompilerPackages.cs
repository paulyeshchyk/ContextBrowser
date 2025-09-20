using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerPackages : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerPackages(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    // context: build, uml
    public async Task<Dictionary<string, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile Packages");

        var contextInfoDataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var contextClassifier = _optionsStore.GetOptions<IDomainPerActionContextTensorClassifier>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.packages.domains.puml");
        var diagramId = $"packages_{outputPath}".AlphanumericOnly();

        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");

        var elements = contextInfoDataset.GetAll();
        int maxNameLength = UmlDiagramMaxNamelengthExtractor.Extract(elements, new() { UmlDiagramMaxNamelengthExtractorType.@method, UmlDiagramMaxNamelengthExtractorType.@namespace, UmlDiagramMaxNamelengthExtractorType.entity, UmlDiagramMaxNamelengthExtractorType.property });

        var classes = elements.Where(e => e.ElementType == ContextInfoElementType.@class).ToList();
        var methods = elements.Where(e => e.ElementType == ContextInfoElementType.method).ToList();

        var grouped = classes.GroupBy(c => c.Namespace);

        foreach (var nsGroup in grouped)
        {
            AddPackage(diagram, methods, nsGroup, maxNameLength);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
        diagram.WriteToFile(outputPath, writeOptons);

        return new Dictionary<string, bool>();
    }

    private static void AddPackage(UmlDiagramClass diagram, List<ContextInfo> methods, IGrouping<string, ContextInfo> nsGroup, int maxnameLength)
    {
        var packageUrl = UmlUrlBuilder.BuildNamespaceUrl(nsGroup.Key);
        var package = new UmlPackage(nsGroup.Key, alias: nsGroup.Key.AlphanumericOnly().PadRight(maxnameLength), url: packageUrl);

        foreach (var cls in nsGroup)
        {
            AddComponentGroup(methods, package, cls, maxnameLength);
        }

        diagram.Add(package);
        diagram.AddRelations(UmlSquaredLayout.Build(methods.Select(m => m.FullName)));
    }

    private static void AddComponentGroup(List<ContextInfo> methods, UmlPackage package, ContextInfo cls, int maxnameLength)
    {
        var stereotype = cls.Contexts.Any()
            ? string.Join(", ", cls.Contexts.OrderBy(c => c))
            : "NoContext";

        var compGroup = new UmlComponentGroup(CleanName(cls.Name.PadRight(maxnameLength)), stereotype);

        var methodsInClass = methods.Where(m => m.ClassOwner?.Name == cls.Name);
        foreach (var method in methodsInClass)
        {
            AddMethodBox(compGroup, method, maxnameLength);
        }

        package.Add(compGroup);
    }

    private static void AddMethodBox(UmlComponentGroup compGroup, ContextInfo method, int maxnameLength)
    {
        var methodStereotype = string.Join(", ", method.Contexts.Distinct().OrderBy(x => x));
        var methodBox = new UmlMethodBox(CleanName(method.Name.PadRight(maxnameLength)), methodStereotype);
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
