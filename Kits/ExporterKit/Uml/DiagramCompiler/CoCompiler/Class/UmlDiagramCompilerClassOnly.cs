using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.Class;

// context: uml, build
public class UmlDiagramCompilerClassOnly : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly UmlClassRendererClassOnly _renderer;

    public UmlDiagramCompilerClassOnly(
        IAppLogger<AppLevel> logger, 
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, 
        IAppOptionsStore optionsStore, 
        INamingProcessor namingProcessor, 
        IUmlUrlBuilder umlUrlBuilder, 
        UmlClassRendererClassOnly renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _renderer = renderer;
    }

    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassOnly");

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var contextInfoDataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var classesOnly = contextInfoDataset.GetAll().Where(c => (c.ElementType.IsEntityDefinition() || c.MethodOwnedByItSelf == true));

        var tasks = classesOnly.Select(async context => await ExportAsync(contextInfo: context,
                                                                       exportOptions: exportOptions,
                                                                   cancellationToken: cancellationToken).ConfigureAwait(false)
        );
        await Task.WhenAll(tasks);
        return new Dictionary<ILabeledValue, bool>();
    }

    //context: uml, build, heatmap, directory
    internal async Task ExportAsync(IContextInfo contextInfo, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var fullName = $"{contextInfo.FullName.AlphanumericOnly()}";

        // грязный хак для получения информации о владельце
        var classownerInfo = contextInfo.MethodOwnedByItSelf
            ? (contextInfo.ClassOwner ?? contextInfo)
            : contextInfo;

        var renderResult = await _renderer.RenderAsync(classownerInfo, cancellationToken).ConfigureAwait(false);

        var classNameWithNameSpace = $"{classownerInfo.Namespace}.{classownerInfo.ShortName}";
        var pumlFileName = _namingProcessor.ClassOnlyPumlFilename(classNameWithNameSpace);
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, pumlFileName);

        var diagramId = _namingProcessor.ClassOnlyDiagramId(fullName);
        var diagramTitle = _namingProcessor.ClassOnlyDiagramTitle(contextInfo.Name);

        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return;
        }
        diagram.DiagramId = diagramId;
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");


        await diagram.WriteToFileAsync(fileName, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);
    }
}
