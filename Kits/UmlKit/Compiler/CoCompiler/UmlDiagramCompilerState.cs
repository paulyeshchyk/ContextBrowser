using System.Collections.Generic;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.DiagramGenerator;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Compiler.CoCompiler;

public class UmlStateDiagramCompilerAction : UmlDiagramCompilerState
{
    protected override FetchType TypeOfFetch => FetchType.FetchAction;

    public UmlStateDiagramCompilerAction(DiagramBuilderOptions options, IContextDiagramBuilder builder, ExportOptions exportOptions, SequenceDiagramGenerator<UmlState> renderer, IAppLogger<AppLevel> logger) : base(options, builder, exportOptions, renderer, logger)
    {
    }
}

public class UmlStateDiagramCompilerDomain : UmlDiagramCompilerState
{
    protected override FetchType TypeOfFetch => FetchType.FetchDomain;

    public UmlStateDiagramCompilerDomain(DiagramBuilderOptions options, IContextDiagramBuilder builder, ExportOptions exportOptions, SequenceDiagramGenerator<UmlState> renderer, IAppLogger<AppLevel> logger) : base(options, builder, exportOptions, renderer, logger)
    {
    }
}

public abstract class UmlDiagramCompilerState
{
    private readonly DiagramBuilderOptions _options;
    private readonly IContextDiagramBuilder _builder;
    private readonly ExportOptions _exportOptions;
    private readonly SequenceDiagramGenerator<UmlState> _renderer;
    private readonly IAppLogger<AppLevel> _logger;

    protected abstract FetchType TypeOfFetch { get; }

    /// <summary>
    /// Создает новый экземпляр компилятора.
    /// </summary>
    /// <param name="options">Опции для построителя диаграммы переходов.</param>
    /// <param name="builder">Фабрика для создания построителя диаграмм.</param>
    /// <param name="exportOptions">Каталог для сохранения выходных файлов.</param>
    /// <param name="renderer">Рендерер переходов.</param>
    /// <param name="logger">Делегат для записи логов.</param>
    protected UmlDiagramCompilerState(
        DiagramBuilderOptions options,
        IContextDiagramBuilder builder,
        ExportOptions exportOptions,
        SequenceDiagramGenerator<UmlState> renderer,
        IAppLogger<AppLevel> logger)
    {
        _options = options;
        _builder = builder;
        _exportOptions = exportOptions;
        _renderer = renderer;
        _logger = logger;
    }

    /// <summary>
    /// Компилирует и рендерит диаграмму состояний для заданного домена и всех контекстов.
    /// </summary>
    /// <param name="metaItem">Имя домена.</param>
    /// <param name="diagramId"></param>
    /// <param name="diagramFileName"></param>
    /// <param name="allContexts">Список всех контекстных элементов.</param>
    /// <param name="cancellationToken"></param>
    public bool CompileAsync(string metaItem, string diagramId, string diagramFileName, List<ContextInfo> allContexts, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Dbg, $"Compile state for [{metaItem}]", LogLevelNode.Start);

        var diagram = new UmlDiagramState(_options, diagramId: diagramId);
        diagram.SetTitle($"Context: {metaItem}");
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);

        // Используем фабрику для создания построителя диаграмм
        var transitions = _builder.Build(metaItem, fetchType: TypeOfFetch, allContexts);

        var rendered = _renderer.Generate(diagram, transitions, metaItem);

        if (rendered)
        {
            // Если рендеринг успешен, записываем диаграмму в файл
            var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
            var path = _exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, diagramFileName);
            diagram.WriteToFile(path, writeOptons);
        }

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return rendered;
    }
}